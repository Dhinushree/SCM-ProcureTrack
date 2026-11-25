
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetAssignmentApp.Data;

namespace DotNetAssignmentApp.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory
        public async Task<IActionResult> Index(int? itemId)
        {
            var query = _context.InventoryLedgers
                .Include(x => x.Item)
                .AsQueryable();

            if (itemId.HasValue)
            {
                query = query.Where(x => x.ItemId == itemId.Value);
            }

            var ledgers = await query
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync();

            ViewBag.Items = await _context.Items
                .OrderBy(x => x.ItemName)
                .ToListAsync();

            ViewBag.SelectedItemId = itemId;

            return View(ledgers);
        }
    }
}
