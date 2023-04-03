using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caa_mis.Data;
using caa_mis.Models;
using caa_mis.ViewModels;
using caa_mis.Utilities;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace caa_mis.Controllers
{
    
    public class EventItemsController : CustomControllers.CognizantController
    {
        private readonly InventoryContext _context;

        public EventItemsController(InventoryContext context)
        {
            _context = context;
        }

        // GET: EventItems
        public async Task<IActionResult> Index(int? EventID, string sortDirectionCheck, string sortFieldID, int? ItemID,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Product Name")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);

            ViewDataReturnURL();

            if (!EventID.HasValue)
            {
                //Go back to the proper return URL for the Transactions controller
                return Redirect(ViewData["returnURL"].ToString());
            }
                        
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Product Name", "Quantity" };

            PopulateDropDownLists();
            
            var item = from a in _context.EventItems
                .Include(b => b.Event)
                .Include(t => t.Item)
                       where a.EventID == EventID.GetValueOrDefault()
                       select a;

            var transactions = _context.Events
                .Include(b => b.Branch)
                .Include(b => b.Employee)
                .Include(b => b.TransactionStatus)
                .Where(p => p.ID == EventID.GetValueOrDefault())
                .AsNoTracking()
                .FirstOrDefault();

            ViewBag.Transactions = transactions;
            
            if (ItemID.HasValue)
            {
                item = item.Where(p => p.ItemID == ItemID);
                ViewData["Filtering"] = "btn-secondary";
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
            if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    item = item
                        .OrderBy(p => p.Quantity);
                }
                else
                {
                    item = item
                        .OrderByDescending(p => p.Quantity);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    item = item
                        .OrderBy(p => p.Item.Name);
                }
                else
                {
                    item = item
                        .OrderByDescending(p => p.Item.Name);
                }
            }
            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for Sorting Options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "EventItems");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<EventItem>.CreateAsync(item.AsNoTracking(), page ?? 1, pageSize);



            return View(pagedData);
        }

        // GET: EventItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewDataReturnURL();
            
            if (id == null || _context.EventItems == null)
            {
                return NotFound();
            }

            var EventItem = await _context.EventItems
                .Include(b => b.Event)
                .Include(b => b.Item)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (EventItem == null)
            {
                return NotFound();
            }

            return View(EventItem);
        }

        // GET: EventItems/Create
        public IActionResult Create()
        {
           
            PopulateDropDownLists();
            
            return View();
        }

        // POST: EventItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProductID,TransactionID,Quantity")] TransactionItemVM transactionItem)
        {
            ViewDataReturnURL();
            
            EventItem bI = new EventItem
            {
                ID = transactionItem.ID,
                EventID = transactionItem.TransactionID,
                ItemID = transactionItem.ProductID,
                Quantity = transactionItem.Quantity
            };

            var itemExists = _context.EventItems
                .Where(p => p.EventID == transactionItem.TransactionID && p.ItemID == transactionItem.ProductID)
                .FirstOrDefault();

            if (itemExists == null)
            {
                if (transactionItem.Quantity > 0)
                {
                    if (validateOnHand(transactionItem.TransactionID, transactionItem.ProductID, transactionItem.Quantity))
                    {
                        if (ModelState.IsValid)
                        {
                            _context.Add(bI);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The changes cannot be saved because the quantity entered is higher than the available stock in the branch.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "The changes cannot be saved. Quantity cannot be negative or 0.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "The changes cannot be saved. There is already an existing product in your list.";
            }
            
            PopulateDropDownLists(bI);
            return Redirect(ViewData["returnURL"].ToString());
        }

        // GET: EventItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewDataReturnURL();
            
            if (id == null || _context.EventItems == null)
            {
                return NotFound();
            }

            var EventItem = await _context.EventItems.FindAsync(id);
            if (EventItem == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(EventItem);
            return View(EventItem);
        }

        // POST: EventItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ItemID,EventID,Quantity")] EventItem EventItem)
        {
            ViewDataReturnURL();
            
            if (id != EventItem.ID)
            {
                return NotFound();
            }
            if (validateOnHand(EventItem.EventID, EventItem.ItemID, EventItem.Quantity))
            {
                if (EventItem.Quantity > 0)
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(EventItem);
                            await _context.SaveChangesAsync();
                            return Redirect(ViewData["returnURL"].ToString());
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!EventItemExists(EventItem.ID))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The changes cannot be saved. Quantity cannot be negative or 0.");
                }
            }
            else
            {
                ModelState.AddModelError("", "The changes cannot be saved because the quantity entered is higher than the available stock in the branch.");
            }
            PopulateDropDownLists(EventItem);
            return View(EventItem);
        }

        // GET: EventItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewDataReturnURL();
            
            if (id == null || _context.EventItems == null)
            {
                return NotFound();
            }

            var EventItem = await _context.EventItems
                .Include(b => b.Event)
                .Include(b => b.Item)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (EventItem == null)
            {
                return NotFound();
            }

            return View(EventItem);
        }

        // POST: EventItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EventItems == null)
            {
                return Problem("Entity set 'InventoryContext.EventItems'  is null.");
            }
            var EventItem = await _context.EventItems.FindAsync(id);
            if (EventItem != null)
            {
                _context.EventItems.Remove(EventItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EventSummary(int? page, int? pageSizeID, int[] BranchID, string sortDirectionCheck,
                                            string sortFieldID, string SearchString, string actionButton, string sortDirection = "asc", string sortField = "BranchName")
        {
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "EmployeeName", "BranchName", "TransactionStatusName", "EventName",
                                            "EventDate", "ItemName", "EventQuantity", "StockQuantity"};

            IQueryable<EventSummaryVM> sumQ = _context.EventSummary;

            if (BranchID != null && BranchID.Length > 0)
            {
                sumQ = sumQ.Where(s => BranchID.Contains(s.BranchID));
                ViewData["Filtering"] = "btn-secondary";
            }            

            if (!String.IsNullOrEmpty(SearchString))
            {
                sumQ = sumQ.Where(i => i.EmployeeName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-secondary";
            }

            ViewData["BranchID"] = BranchList(BranchID);            

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
            if (sortField == "EmployeeName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EmployeeName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.EmployeeName);
                }
            }
            else if (sortField == "BranchName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.BranchName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.BranchName);
                }
            }
            
            else if (sortField == "TransactionStatusName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.TransactionStatusName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.TransactionStatusName);
                }
            }
            else if (sortField == "EventName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EventName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.EventName);
                }
            }
            else if (sortField == "EventDate")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EventDate);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.EventDate);
                }
            }
            else if (sortField == "ItemName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName);
                }
            }
            else if (sortField == "EventQuantity")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EventQuantity);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.BranchName)
                        .ThenBy(p => p.EventDate);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.BranchName)
                        .ThenByDescending(p => p.EventDate);
                }
            }

            // Save filtered data to cookie
            CachingFilteredData(sumQ);

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "EventSummary");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<EventSummaryVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }
        public IActionResult DownloadEventItems()
        {
            //retrieving filtered data from cookie
            var items = JsonConvert.DeserializeObject<IEnumerable<EventSummaryVM>>(
            Request.Cookies["filteredData"]);

            int numRows = items.Count();

            if (numRows > 0)
            {
                using ExcelPackage excel = new();
                var workSheet = excel.Workbook.Worksheets.Add("EventProducts");

                workSheet.Cells[3, 1].LoadFromCollection(items, true);
                workSheet.Column(10).Style.Numberformat.Format = "yyyy-mm-dd";
                //Set Style and backgound colour of headings
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 13])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightCyan);
                }

                //Autofit columns
                workSheet.Cells.AutoFitColumns();

                //Add a title and timestamp at the top of the report
                workSheet.Cells[1, 1].Value = "Event Product Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 13])
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
                using (ExcelRange Rng = workSheet.Cells[2, 13])
                {
                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                        localDate.ToShortDateString();

                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                try
                {
                    Byte[] theData = excel.GetAsByteArray();
                    string filename = "EventProducts.xlsx";
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

        public bool validateOnHand(int TransactionID, int ProductID, int Quantity)
        {
            //validate quantity vs the item
            var trans = _context.Events
                .FirstOrDefault(p => p.ID == TransactionID);
            
            if(trans.BranchID == 1)
            {
                return true;
            }
            
            var currentOnHand = _context.Stocks
                .Where(p => p.ItemID == ProductID && p.BranchID == trans.BranchID)
                .FirstOrDefault();

            if (currentOnHand == null)
            {
                return false;
            }
            
            return currentOnHand.Quantity >= Quantity;
        }

        private bool EventItemExists(int id)
        {
          return _context.EventItems.Any(e => e.ID == id);
        }

        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private SelectList ItemList(int? selectedId)
        {
            return new SelectList(_context
                .Items
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }
        private SelectList BranchList(int[] selectedId)
        {
            return new SelectList(_context.Branches
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }
        private void CachingFilteredData<T>(IQueryable<T> sumQ)
        {
            FilteredDataCaching.SaveFilteredData(HttpContext, "filteredData", sumQ, 120);
        }

        private SelectList TransactionList(int? selectedId)
        {
            return new SelectList(_context
                .Events
                .OrderBy(m => m.Branch.Name), "ID", "Name", selectedId);
        }
        private void PopulateDropDownLists(EventItem EventItem = null)
        {
            ViewData["ItemID"] = ItemList(EventItem?.ItemID);
            ViewData["EventID"] = TransactionList(EventItem?.EventID);

        }
    }
}
