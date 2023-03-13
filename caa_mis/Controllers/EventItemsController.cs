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

            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-primary";

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
            
            if (ModelState.IsValid)
            {
                _context.Add(bI);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(bI);
            return View(bI);
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
