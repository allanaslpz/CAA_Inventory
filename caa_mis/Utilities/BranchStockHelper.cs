using caa_mis.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caa_mis.Utilities
{
    public class BranchStockHelper
    {
        private readonly InventoryContext _context;
        public BranchStockHelper(InventoryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetBranchStock(int itemID)
        {
            var inventory = await _context.Stocks
                .Include(s => s.Branch).ThenInclude(s =>s.Stocks)
                .ToListAsync();

            return (IActionResult)inventory;
        }
    }
}
