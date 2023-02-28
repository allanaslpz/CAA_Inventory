using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caa_mis.Data;
using caa_mis.Models;
using caa_mis.Utilities;
using caa_mis.ViewModels;
using Newtonsoft.Json;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

namespace caa_mis.Controllers
{
    public class TransactionsController : CustomControllers.CognizantController
    {
        private readonly InventoryContext _context;

        public TransactionsController(InventoryContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index(string sortDirectionCheck, string sortFieldID, string SearchString, int? TransactionTypeID, int? TransactionStatusID, int? DestinationID,
            int? page, int? pageSizeID, string actionButton, string sortDirection = "asc", string sortField = "Type")
        {
            //Clear the sort/filter/paging URL Cookie for Controller
            CookieHelper.CookieSet(HttpContext, ControllerName() + "URL", "", -1);
            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-primary";

            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Type", "Description", "Origin", "Destination", "Transaction Date", "Transaction Status" };

            PopulateDropDownLists();

            var inventory = _context.Transactions
                .Include(t => t.Destination)
                .Include(t => t.Employee)
                .Include(t => t.Origin)
                .Include(t => t.TransactionStatus)
                .Include(t => t.TransactionType)
                .AsNoTracking();

            if (TransactionTypeID.HasValue)
            {
                inventory = inventory.Where(p => p.TransactionTypeID == TransactionTypeID);
                ViewData["Filtering"] = "btn-danger";
            }
            if (TransactionStatusID.HasValue)
            {
                inventory = inventory.Where(p => p.TransactionTypeID == TransactionTypeID);
                ViewData["Filtering"] = "btn-danger";
            }
            if (DestinationID.HasValue)
            {
                inventory = inventory.Where(p => p.DestinationID == DestinationID);
                ViewData["Filtering"] = "btn-danger";
            }
            if (!String.IsNullOrEmpty(SearchString))
            {
                inventory = inventory.Where(p => p.Description.ToUpper().Contains(SearchString.ToUpper()));
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
            if (sortField == "Type")
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderBy(p => p.TransactionType.Name);
                }
                else
                {
                    inventory = inventory
                        .OrderByDescending(p => p.TransactionType.Name);
                }
            }
            else if (sortField == "Description")
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderByDescending(p => p.Description);
                }
                else
                {
                    inventory = inventory
                        .OrderBy(p => p.Description);
                }
            }
            else if (sortField == "Origin")
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderBy(p => p.Origin.Name);
                }
                else
                {
                    inventory = inventory
                        .OrderByDescending(p => p.Origin.Name);
                }
            }
            else if (sortField == "Destination")
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderBy(p => p.Destination.Name);
                }
                else
                {
                    inventory = inventory
                        .OrderByDescending(p => p.Destination.Name);
                }
            }
            else if (sortField == "Transaction Date")
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderBy(p => p.TransactionDate);
                }
                else
                {
                    inventory = inventory
                        .OrderByDescending(p => p.TransactionDate);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    inventory = inventory
                        .OrderBy(p => p.TransactionStatus.Name);
                }
                else
                {
                    inventory = inventory
                        .OrderByDescending(p => p.TransactionStatus.Name);
                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for Sorting Options
            ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            //Handle Paging
            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "Transactions");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<Transaction>.CreateAsync(inventory.AsNoTracking(), page ?? 1, pageSize);


            return View(pagedData);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Destination)
                .Include(t => t.Employee)
                .Include(t => t.Origin)
                .Include(t => t.TransactionStatus)
                .Include(t => t.TransactionType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            PopulateDropDownLists();
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,EmployeeID,TransactionStatusID,TransactionTypeID,OriginID,DestinationID,TransactionDate,ReceivedDate,Description,Shipment")] Transaction transaction)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "TransactionItems", new { TransactionID = transaction.ID });
            }
            PopulateDropDownLists(transaction);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            PopulateDropDownLists(transaction);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,EmployeeID,TransactionStatusID,TransactionTypeID,OriginID,DestinationID,TransactionDate,ReceivedDate,Description,Shipment")] Transaction transaction)
        {
            if (id != transaction.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "TransactionItems", new { TransactionID = transaction.ID });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }                
            }
            PopulateDropDownLists(transaction);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Destination)
                .Include(t => t.Employee)
                .Include(t => t.Origin)
                .Include(t => t.TransactionStatus)
                .Include(t => t.TransactionType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'InventoryContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> TransactionSummary(int? page, int? pageSizeID, int[] OriginID, int[] DestinationID, string sortDirectionCheck,
                                            string sortFieldID, string SearchString, string actionButton, string sortDirection = "asc", string sortField = "OriginName")
        {
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "EmployeeName", "OriginName", "DestinationName", "TransactionStatusName", "TransactionTypeName",
                                            "TransactionDate", "ReceivedDate", "Description", "Shipment"};

            IQueryable<TransactionSummaryVM> sumQ = _context.TransactionSummary;

            if (OriginID != null && OriginID.Length > 0)
            {
                sumQ = sumQ.Where(s => OriginID.Contains(s.OriginID));
                ViewData["Filtering"] = "btn-danger";
            }
            if (DestinationID != null && DestinationID.Length > 0)
            {
                sumQ = sumQ.Where(s => DestinationID.Contains(s.OriginID));
                ViewData["Filtering"] = "btn-danger";
            }

            if (!String.IsNullOrEmpty(SearchString))
            {
                sumQ = sumQ.Where(i => i.EmployeeName.ToUpper().Contains(SearchString.ToUpper()));
                ViewData["Filtering"] = "btn-danger";
            }

            ViewData["OriginID"] = BranchList(OriginID);
            ViewData["DestinationID"] = BranchList(DestinationID);
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
            else if (sortField == "OriginName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.OriginName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.OriginName);
                }
            }
            else if (sortField == "DestinationName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.DestinationName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.DestinationName);
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
            else if (sortField == "TransactionTypeName")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.TransactionTypeName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.TransactionTypeName);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.OriginName)
                        .ThenBy(p => p.DestinationName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.OriginName)
                        .ThenByDescending(p => p.DestinationName);
                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for Sorting Options
            //ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "TransactionSummary");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<TransactionSummaryVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }
        private bool TransactionExists(int id)
        {
          return _context.Transactions.Any(e => e.ID == id);
        }

        private void ViewDataReturnURL()
        {
            ViewData["returnURL"] = MaintainURL.ReturnURL(HttpContext, ControllerName());
        }

        private SelectList DestinationSelectList(int? selectedId)
        {
            return new SelectList(_context
                .Branches
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }
        private SelectList EmployeeList(int? selectedId)
        {
            return new SelectList(_context
                .Employees
                .OrderBy(m => m.FirstName), "ID", "FirstName", selectedId);
        }
        private SelectList OriginList(int? selectedId)
        {
            return new SelectList(_context
                .Branches
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }
        private SelectList TransactionStatusList(int? selectedId)
        {
            return new SelectList(_context
                .TransactionStatuses
                .OrderBy(m => m.Name), "ID", "Name", selectedId);
        }

        private SelectList TransactionTypeList(int? selectedId)
        {
            var a = _context.TransactionTypes.Select(s => new
            {
                s.ID,
                Name = s.Name + " - Stock " + s.InOut.ToString(),
                Name2 = s.Name
            });
            return new SelectList(a
                .OrderBy(s => s.Name2), "ID", "Name", selectedId);
            /*return new SelectList(_context
                .TransactionTypes
                .OrderBy(m => m.Name), "ID", "Name", selectedId);*/
        }
        private SelectList BranchList(int[] selectedId)
        {
            return new SelectList(_context.Branches
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }

        private void PopulateDropDownLists(Transaction transaction = null)
        {
            ViewData["DestinationID"] = DestinationSelectList(transaction?.DestinationID);
            ViewData["EmployeeID"] = EmployeeList(transaction?.EmployeeID);
            ViewData["OriginID"] = OriginList(transaction?.OriginID);
            ViewData["TransactionStatusID"] = TransactionStatusList(transaction?.TransactionStatusID);
            ViewData["TransactionTypeID"] = TransactionTypeList(transaction?.TransactionTypeID);
        }
        private void CachingFilteredData<T>(IQueryable<T> sumQ)
        {
            FilteredDataCaching.SaveFilteredData(HttpContext, "filteredData", sumQ, 120);
        }
        public IActionResult DownloadTransactions()
        {
            //retrieving filtered data from cookie
            var items = JsonConvert.DeserializeObject<IEnumerable<TransactionSummaryVM>>(
            Request.Cookies["filteredData"]);

            int numRows = items.Count();

            if (numRows > 0)
            {
                using ExcelPackage excel = new();
                var workSheet = excel.Workbook.Worksheets.Add("ProductsTransaction");

                workSheet.Cells[3, 1].LoadFromCollection(items, true);

                //Set Style and backgound colour of headings
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 15])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightCyan);
                }

                //Autofit columns
                workSheet.Cells.AutoFitColumns();

                //Add a title and timestamp at the top of the report
                workSheet.Cells[1, 1].Value = "Product Transaction Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 15])
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
                using (ExcelRange Rng = workSheet.Cells[2, 15])
                {
                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                        localDate.ToShortDateString();

                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                try
                {
                    Byte[] theData = excel.GetAsByteArray();
                    string filename = "ProductsTransaction.xlsx";
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
