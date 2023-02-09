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
            ViewData["Filtering"] = "btn-outline-secondary";

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
                return RedirectToAction(nameof(Index));
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
                ID = s.ID,
                Name = s.Name + " - Stock " + s.InOut.ToString(),
                Name2 = s.Name
            });
            return new SelectList(a
                .OrderBy(s => s.Name2), "ID", "Name", selectedId);
            /*return new SelectList(_context
                .TransactionTypes
                .OrderBy(m => m.Name), "ID", "Name", selectedId);*/
        }
        private void PopulateDropDownLists(Transaction transaction = null)
        {
            ViewData["DestinationID"] = DestinationSelectList(transaction?.DestinationID);
            ViewData["EmployeeID"] = EmployeeList(transaction?.EmployeeID);
            ViewData["OriginID"] = OriginList(transaction?.OriginID);
            ViewData["TransactionStatusID"] = TransactionStatusList(transaction?.TransactionStatusID);
            ViewData["TransactionTypeID"] = TransactionTypeList(transaction?.TransactionTypeID);
        }
    }
}
