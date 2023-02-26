using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caa_mis.Data;
using caa_mis.Models;
using SkiaSharp;
using caa_mis.Utilities;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using caa_mis.ViewModels;
using Microsoft.CodeAnalysis.Operations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Newtonsoft.Json;

namespace caa_mis.Controllers
{
    public class ItemsController : CustomControllers.CognizantController
    {
        private readonly InventoryContext _context;
        public ItemsController(InventoryContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index(string sortDirectionCheck, string sortFieldID, string SearchString, string SearchSKU, int? CategoryID, int? ItemStatusID,
            int? ManufacturerID, int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Name")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-primary";

            PopulateDropDownLists();

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Name", "Category", "SKUNumber", "Cost" };

            //by default we want to show the active
            
            var inventory = _context.Items
                                    .Include(i => i.Category)
                                    .Include(i => i.ItemStatus)
                                    .Include(i => i.ItemThumbnail)
                                    .Include(i => i.Manufacturer)
                                    .AsNoTracking();

            //Add as many filters as needed
            if (CategoryID.HasValue)
            {
                inventory = inventory.Where(p => p.CategoryID == CategoryID);
                ViewData["Filtering"] = "btn-danger";
            }
            if (ItemStatusID.HasValue)
            {
                inventory = inventory.Where(p => p.ItemStatusID == ItemStatusID);
                ViewData["Filtering"] = "btn-danger";
            }
            else
            {
                inventory = inventory.Where(p => p.ItemStatusID != 2);
            }
            if (ManufacturerID.HasValue)
            {
                inventory = inventory.Where(p => p.ManufacturerID == ManufacturerID);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                inventory = inventory.Where(p => p.Name.ToUpper().Contains(SearchString.ToUpper())
                                       || p.Description.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchSKU))
            {
                inventory = inventory.Where(p => p.SKUNumber.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
                else //Sort by the controls in the filter area
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }

            //Now we know which field and direction to sort by
            if (sortField == "SKUNumber")
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderBy(p => p.SKUNumber);
                }
                else
                {
                    inventory = inventory
                        .OrderByDescending(p => p.SKUNumber);
                }
            }
            else if (sortField == "Cost")
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderByDescending(p => p.Cost);
                }
                else
                {
                    inventory = inventory
                        .OrderBy(p => p.Cost);
                }
            }
            else if (sortField == "Category")
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderBy(p => p.Category.Name);
                }
                else
                {
                    inventory = inventory
                        .OrderByDescending(p => p.Category.Name);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderBy(p => p.Name)
                        .ThenBy(p => p.Description);
                }
                else
                {
                    inventory = inventory
                        .OrderByDescending(p => p.Name)
                        .ThenByDescending(p => p.Description);
                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for Sorting Options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            // Save filtered data to cookie
            CachingFilteredData(inventory);

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Items");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Item>.CreateAsync(inventory.AsNoTracking(), page ?? 1, pageSize);


            return View(pagedData);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Manufacturer)
                .Include(i => i.ItemStatus)
                .Include(i => i.ItemPhoto)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewDataReturnURL();

            PopulateDropDownLists();
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CategoryID,SKUNumber,Name,Description,Scale,Cost,ManufacturerID,ItemStatusID")] Item item, IFormFile thePicture)
        {
            ViewDataReturnURL();

            try
            {
                if (ModelState.IsValid)
                {
                    await AddPicture(item, thePicture);
                    _context.Add(item);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes after multiple attempts. Try again, and if the problem persists, see your system administrator.");
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE constraint failed: SKUNumber"))
                {
                    ModelState.AddModelError("SKUNumber", "Unable to save changes. Remember, you cannot have duplicate SKU numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            PopulateDropDownLists(item);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();

            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.ItemPhoto)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (item == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(item);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string chkRemoveImage, IFormFile thePicture)
        {
            //URL with the last filter, sort and page parameters for this controller
            ViewDataReturnURL();


            var itemToUpdate = await _context.Items
                .Include(i => i.ItemPhoto)
                .FirstOrDefaultAsync(i => i.ID == id);

            if (itemToUpdate == null)
            {
                return NotFound();
            }
           
            if (await TryUpdateModelAsync<Item>(itemToUpdate, "",
                i => i.CategoryID, i => i.SKUNumber, i => i.Name, i => i.Description, i => i.Scale, i => i.Cost, i => i.ManufacturerID, i => i.ItemStatusID))
            {
                try
                {
                    //For the image
                    if (chkRemoveImage != null)
                    {
                        //If we are just deleting the two versions of the photo, we need to make sure the Change Tracker knows
                        //about them both so go get the Thumbnail since we did not include it.
                        itemToUpdate.ItemThumbnail = _context.ItemThumbnails.Where(p => p.ItemID == itemToUpdate.ID).FirstOrDefault();
                        //Then, setting them to null will cause them to be deleted from the database.
                        itemToUpdate.ItemPhoto = null;
                        itemToUpdate.ItemThumbnail = null;
                    }
                    else
                    {
                        await AddPicture(itemToUpdate, thePicture);
                    }
                    //_context.Update(item);
                    await _context.SaveChangesAsync();
                    return Redirect(ViewData["returnURL"].ToString());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(itemToUpdate.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(itemToUpdate);
            return View(itemToUpdate);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewDataReturnURL();

            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.ItemStatus)
                .Include(i => i.Manufacturer)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'InventoryContext.Items'  is null.");
            }
            var item = await _context.Items
                .Include(i => i.ItemPhoto)
                .FirstOrDefaultAsync(i => i.ID == id);
            
            item.ItemStatusID = 2; //Set the status to "Discontinued";
            
            if (item != null)
            {
                await _context.SaveChangesAsync();
            }

            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> StockSummaryByBranch(int? page, int? pageSizeID, int[] BranchID, string sortDirectionCheck,
                                            string sortFieldID, string SearchString, string actionButton, string sortDirection = "asc", string sortField = "BranchName")
        {
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "BranchName", "ItemName", "ItemCost", "Quantity", "MinLevel" };

            IQueryable<StockSummaryByBranchVM> sumQ = _context.StockSummaryByBranch;

            if (BranchID != null && BranchID.Length > 0)
            {
                sumQ = sumQ.Where(s => BranchID.Contains(s.BranchID));
                ViewData["Filtering"] = "btn-danger";
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                sumQ = sumQ.Where(i => i.ItemName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }

            ViewData["BranchID"] = BranchList(BranchID);
            // Save filtered data to cookie
            CachingFilteredData(sumQ);

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
                else //Sort by the controls in the filter area
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }

            //Now we know which field and direction to sort by
            if (sortField == "BranchName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.BranchName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.BranchName);
                }
            }
            else if (sortField == "ItemName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemName);
                }
            }
            else if (sortField == "ItemCost")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemCost);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemCost);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.Quantity);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.Quantity);
                }
            }
            else if (sortField == "MinLevel")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.MinLevel);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.MinLevel);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemName)
                        .ThenBy(p => p.Quantity);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName)
                        .ThenByDescending(p => p.Quantity);
                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for Sorting Options
            //ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "StockItemSummary");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<StockSummaryByBranchVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }
        private SelectList CategorySelectList(int? selectedId)
        {
            return new SelectList(_context.Categories
                .OrderBy(d => d.Name)
                .ThenBy(d => d.Description), "ID", "Name", selectedId);
        }

        private SelectList ItemStatusList(int? selectedId)
        {
            return new SelectList(_context
                .ItemStatuses
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }

        private SelectList BranchList(int[] selectedId)
        {
            return new SelectList(_context.Branches
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }

        private SelectList ManufacturerList(int? selectedId)
        {
            return new SelectList(_context
                .Manufacturers
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }

        private void PopulateDropDownLists(Item item = null)
        {
            ViewData["CategoryID"] = CategorySelectList(item?.CategoryID);
            ViewData["ItemStatusID"] = ItemStatusList(item?.ItemStatusID);
            ViewData["ManufacturerID"] = ItemStatusList(item?.ManufacturerID);
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.ID == id);
        }

        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private void CachingFilteredData<T>(IQueryable<T> sumQ)
        {
            FilteredDataCaching.SaveFilteredData(HttpContext, "filteredData", sumQ, 120);
        }
        private async Task AddPicture(Item item, IFormFile thePicture)
        {
            //Get the picture and save it with the Patient (2 sizes)
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0))//Looks like we have a file!!!
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray();//Gives us the Byte[]

                        //Check if we are replacing or creating new
                        if (item.ItemPhoto != null)
                        {
                            //We already have pictures so just replace the Byte[]
                            item.ItemPhoto.Content = ResizeImage.shrinkImageWebp(pictureArray, 500, 600);

                            //Get the Thumbnail so we can update it.  Remember we didn't include it
                            item.ItemThumbnail = _context.ItemThumbnails.Where(p => p.ItemID == item.ID).FirstOrDefault();
                            item.ItemThumbnail.Content = ResizeImage.shrinkImageWebp(pictureArray, 75, 90);
                        }
                        else //No pictures saved so start new
                        {
                            item.ItemPhoto = new ItemPhoto
                            {
                                Content = ResizeImage.shrinkImageWebp(pictureArray, 500, 600),
                                MimeType = "image/webp"
                            };
                            item.ItemThumbnail = new ItemThumbnail
                            {
                                Content = ResizeImage.shrinkImageWebp(pictureArray, 75, 90),
                                MimeType = "image/webp"
                            };
                        }
                    }
                }
            }
        }

        public IActionResult DownloadItems()
        {
            //retrieving filtered data from cookie
            var filteredData = JsonConvert.DeserializeObject<IEnumerable<Item>>(
            Request.Cookies["filteredData"]);

            var items = from a in filteredData
                        orderby a.Name ascending
                        select new
                        {
                            a.ID,
                            SKU_Number = a.SKUNumber,
                            Item = a.Name,
                            a.Description,
                            a.CategoryID,
                            a.ItemStatusID,
                            a.Cost,
                        };
            int numRows = items.Count();

            if (numRows > 0)
            {
                using ExcelPackage excel = new();
                var workSheet = excel.Workbook.Worksheets.Add("Products");

                workSheet.Cells[3, 1].LoadFromCollection(items, true);
                //Style fee column for currency
                workSheet.Column(7).Style.Numberformat.Format = "###,##0.00";

                //Make Date and Item Bold
                workSheet.Cells[4, 3, numRows + 3, 3].Style.Font.Bold = true;

                using (ExcelRange totalcost = workSheet.Cells[numRows + 4, 7])
                {
                    totalcost.Formula = "Sum(" + workSheet.Cells[4, 7].Address + ":" + workSheet.Cells[numRows + 3, 7].Address + ")";
                    totalcost.Style.Font.Bold = true;
                    totalcost.Style.Numberformat.Format = "$###,##0.00";
                }

                //Set Style and backgound colour of headings
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightCyan);
                }

                //Autofit columns
                workSheet.Cells.AutoFitColumns();


                //Add a title and timestamp at the top of the report
                workSheet.Cells[1, 1].Value = "Product Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 7])
                {
                    Rng.Merge = true; //Merge columns start and end range
                    Rng.Style.Font.Bold = true; //Font should be bold
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                //Since the time zone where the server is running can be different, adjust to 
                //Local for us.
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 6])
                {
                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                        localDate.ToShortDateString();

                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }


                try
                {
                    Byte[] theData = excel.GetAsByteArray();
                    string filename = "Products.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(theData, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not build and download the file.");
                }
            }
            return NotFound("No data.");
        }
        public IActionResult DownloadStockItems()
        {
            //retrieving filtered data from cookie
            var items = JsonConvert.DeserializeObject<IEnumerable<StockSummaryByBranchVM>>(
            Request.Cookies["filteredData"]);

            int numRows = items.Count();

            if (numRows > 0)
            {
                using ExcelPackage excel = new();
                var workSheet = excel.Workbook.Worksheets.Add("StockProducts");

                workSheet.Cells[3, 1].LoadFromCollection(items, true);

                //Set Style and backgound colour of headings
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightCyan);
                }

                //Autofit columns
                workSheet.Cells.AutoFitColumns();

                //Add a title and timestamp at the top of the report
                workSheet.Cells[1, 1].Value = "Stock Product Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 7])
                {
                    Rng.Merge = true; //Merge columns start and end range
                    Rng.Style.Font.Bold = true; //Font should be bold
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                //Since the time zone where the server is running can be different, adjust to 
                //Local for us.
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 7])
                {
                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                        localDate.ToShortDateString();

                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                try
                {
                    Byte[] theData = excel.GetAsByteArray();
                    string filename = "StockProducts.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(theData, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not build and download the file.");
                }
            }
            return NotFound("No data.");
        }

    }
}