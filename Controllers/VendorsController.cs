
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DotNetAssignmentApp.Data;
using DotNetAssignmentApp.Models;

namespace DotNetAssignmentApp.Controllers
{
    [Authorize(Policy = "ProcurementAccess")]
    public class VendorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vendors
        public async Task<IActionResult> Index()
        {
            var vendors = await _context.Vendors
                .OrderBy(v => v.CompanyName)
                .ToListAsync();

            return View(vendors);
        }

        // GET: Vendors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vendors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                bool exists = await _context.Vendors
                    .AnyAsync(v => v.VendorCode == vendor.VendorCode);

                if (exists)
                {
                    ModelState.AddModelError(
                        "VendorCode", "Vendor Code already exists.");

                    return View(vendor);
                }

                _context.Vendors.Add(vendor);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(vendor);
        }

        // GET: Vendors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
                return NotFound();

            return View(vendor);
        }

        // POST: Vendors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vendor vendor)
        {
            if (id != vendor.VendorId)
                return NotFound();

            if (ModelState.IsValid)
            {
                bool duplicate = await _context.Vendors
                    .AnyAsync(v => v.VendorCode == vendor.VendorCode
                                && v.VendorId != vendor.VendorId);

                if (duplicate)
                {
                    ModelState.AddModelError(
                        "VendorCode", "Vendor Code already exists.");

                    return View(vendor);
                }

                var existingVendor = await _context.Vendors.FindAsync(id);

                if (existingVendor == null)
                    return NotFound();

                existingVendor.VendorCode = vendor.VendorCode;
                existingVendor.CompanyName = vendor.CompanyName;
                existingVendor.ContactPerson = vendor.ContactPerson;
                existingVendor.Email = vendor.Email;
                existingVendor.Phone = vendor.Phone;
                existingVendor.TaxGSTId = vendor.TaxGSTId;
                existingVendor.PaymentTerms = vendor.PaymentTerms;
                existingVendor.IsActive = vendor.IsActive;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(vendor);
        }

        // POST: Vendors/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
                return NotFound();

            if (vendor.IsActive)
            {
                bool hasOpenOrders = await _context.PurchaseOrders
                    .AnyAsync(po => po.VendorId == id &&
                        (po.Status == "Draft" ||
                         po.Status == "Approved" ||
                         po.Status == "Partially Received"));

                if (hasOpenOrders)
                {
                    TempData["Error"] =
                        "Cannot deactivate vendor because open purchase orders exist.";

                    return RedirectToAction(nameof(Index));
                }
            }

            vendor.IsActive = !vendor.IsActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
