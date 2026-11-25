
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DotNetAssignmentApp.Data;
using DotNetAssignmentApp.Models;

namespace DotNetAssignmentApp.Controllers
{
    [Authorize(Policy = "ProcurementAccess")]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items
                .OrderBy(i => i.ItemName)
                .ToListAsync();

            return View(items);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item)
        {
            if (ModelState.IsValid)
            {
                bool exists = await _context.Items
                    .AnyAsync(i => i.ItemCode == item.ItemCode);

                if (exists)
                {
                    ModelState.AddModelError(
                        "ItemCode", "Item Code already exists.");

                    return View(item);
                }

                _context.Items.Add(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var item = await _context.Items.FindAsync(id);

            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Item item)
        {
            if (id != item.ItemId)
                return NotFound();

            if (ModelState.IsValid)
            {
                bool duplicate = await _context.Items
                    .AnyAsync(i => i.ItemCode == item.ItemCode
                                && i.ItemId != item.ItemId);

                if (duplicate)
                {
                    ModelState.AddModelError(
                        "ItemCode", "Item Code already exists.");

                    return View(item);
                }

                var existingItem = await _context.Items.FindAsync(id);

                if (existingItem == null)
                    return NotFound();

                existingItem.ItemCode = item.ItemCode;
                existingItem.ItemName = item.ItemName;
                existingItem.Category = item.Category;
                existingItem.UnitOfMeasure = item.UnitOfMeasure;
                existingItem.ReorderLevel = item.ReorderLevel;
                existingItem.CurrentStock = item.CurrentStock;
                existingItem.IsActive = item.IsActive;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // POST: Items/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
                return NotFound();

            item.IsActive = !item.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
