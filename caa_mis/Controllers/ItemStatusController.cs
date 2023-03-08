using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caa_mis.Data;
using caa_mis.Models;
using Microsoft.AspNetCore.Authorization;

namespace caa_mis.Controllers
{
    [Authorize(Roles = "Admin, Supervisor")]
    public class ItemStatusController : Controller
    {
        private readonly InventoryContext _context;

        public ItemStatusController(InventoryContext context)
        {
            _context = context;
        }

        // GET: ItemStatus
        public async Task<IActionResult> Index()
        {
              return View(await _context.ItemStatuses.ToListAsync());
        }

        // GET: ItemStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ItemStatuses == null)
            {
                return NotFound();
            }

            var itemStatus = await _context.ItemStatuses
                .FirstOrDefaultAsync(m => m.ID == id);
            if (itemStatus == null)
            {
                return NotFound();
            }

            return View(itemStatus);
        }

        // GET: ItemStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ItemStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Description")] ItemStatus itemStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(itemStatus);
        }

        // GET: ItemStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ItemStatuses == null)
            {
                return NotFound();
            }

            var itemStatus = await _context.ItemStatuses.FindAsync(id);
            if (itemStatus == null)
            {
                return NotFound();
            }
            return View(itemStatus);
        }

        // POST: ItemStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Description")] ItemStatus itemStatus)
        {
            if (id != itemStatus.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemStatusExists(itemStatus.ID))
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
            return View(itemStatus);
        }

        // GET: ItemStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ItemStatuses == null)
            {
                return NotFound();
            }

            var itemStatus = await _context.ItemStatuses
                .FirstOrDefaultAsync(m => m.ID == id);
            if (itemStatus == null)
            {
                return NotFound();
            }

            return View(itemStatus);
        }

        // POST: ItemStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ItemStatuses == null)
            {
                return Problem("Entity set 'InventoryContext.ItemStatuses'  is null.");
            }
            var itemStatus = await _context.ItemStatuses.FindAsync(id);
            if (itemStatus != null)
            {
                _context.ItemStatuses.Remove(itemStatus);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemStatusExists(int id)
        {
          return _context.ItemStatuses.Any(e => e.ID == id);
        }
    }
}
