
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using DotNetAssignmentApp.Data;
using DotNetAssignmentApp.Models;
using DotNetAssignmentApp.ViewModels;

namespace DotNetAssignmentApp.Controllers
{
    [Authorize(Policy = "ProcurementAccess")]
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrders
        // Supports Vendor, Date Range and Status filters
        public async Task<IActionResult> Index(
            int? vendorId,
            DateTime? fromDate,
            DateTime? toDate,
            string? status)
        {
            var query = _context.PurchaseOrders
                .Include(po => po.Vendor)
                .AsQueryable();

            // Vendor filter
            if (vendorId.HasValue && vendorId.Value > 0)
            {
                query = query.Where(po =>
                    po.VendorId == vendorId.Value);
            }

            // Date range validation
            bool validDateRange =
                !fromDate.HasValue ||
                !toDate.HasValue ||
                fromDate.Value.Date <= toDate.Value.Date;

            if (validDateRange)
            {
                if (fromDate.HasValue)
                {
                    query = query.Where(po =>
                        po.OrderDate >= fromDate.Value.Date);
                }

                if (toDate.HasValue)
                {
                    var exclusiveEndDate =
                        toDate.Value.Date.AddDays(1);

                    query = query.Where(po =>
                        po.OrderDate < exclusiveEndDate);
                }
            }

            // PO Status filter
            var allowedStatuses = new[]
            {
                "Draft",
                "Approved",
                "Partially Received",
                "Completed",
                "Cancelled"
            };

            if (!string.IsNullOrWhiteSpace(status) &&
                allowedStatuses.Contains(status))
            {
                query = query.Where(po =>
                    po.Status == status);
            }

            var orders = await query
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();

            // Load filter dropdowns and preserve selections
            await LoadFilterDropdowns(vendorId, status);

            ViewBag.VendorId = vendorId;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedStatus = status;

            if (!validDateRange)
            {
                ViewBag.FilterError =
                    "From Date cannot be later than To Date.";
            }

            return View(orders);
        }

        // GET: PurchaseOrders/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();

            var model = new PurchaseOrderViewModel
            {
                OrderDate = DateTime.Today,
                ExpectedDeliveryDate = DateTime.Today.AddDays(7)
            };

            return View(model);
        }

        // POST: PurchaseOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            PurchaseOrderViewModel model)
        {
            model.Items ??= new List<PurchaseOrderItemViewModel>();

            if (!model.Items.Any())
            {
                ModelState.AddModelError(
                    "",
                    "Add at least one item to the Purchase Order."
                );
            }

            if (ModelState.IsValid)
            {
                var vendorExists = await _context.Vendors
                    .AnyAsync(v =>
                        v.VendorId == model.VendorId &&
                        v.IsActive
                    );

                if (!vendorExists)
                {
                    ModelState.AddModelError(
                        "VendorId",
                        "Select a valid active vendor."
                    );
                }

                var itemIds = model.Items
                    .Select(x => x.ItemId)
                    .Distinct()
                    .ToList();

                var validItemCount = await _context.Items
                    .CountAsync(i =>
                        itemIds.Contains(i.ItemId) &&
                        i.IsActive
                    );

                if (validItemCount != itemIds.Count)
                {
                    ModelState.AddModelError(
                        "",
                        "Select valid active items."
                    );
                }

                if (model.ExpectedDeliveryDate.Date <
                    model.OrderDate.Date)
                {
                    ModelState.AddModelError(
                        "ExpectedDeliveryDate",
                        "Expected delivery date cannot be before order date."
                    );
                }
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            decimal subtotal = 0;
            decimal taxAmount = 0;

            var orderDetails = new List<PurchaseOrderDetail>();

            foreach (var item in model.Items)
            {
                decimal amount =
                    item.OrderedQuantity * item.UnitPrice;

                decimal tax =
                    amount * item.TaxPercentage / 100;

                subtotal += amount;
                taxAmount += tax;

                orderDetails.Add(new PurchaseOrderDetail
                {
                    ItemId = item.ItemId,
                    OrderedQuantity = item.OrderedQuantity,
                    UnitPrice = item.UnitPrice,
                    TaxPercentage = item.TaxPercentage,
                    Amount = amount + tax
                });
            }

            var purchaseOrder = new PurchaseOrder
            {
                PONumber = "PO-" +
                    DateTime.Now.ToString("yyyyMMddHHmmssfff"),

                VendorId = model.VendorId,
                OrderDate = model.OrderDate,
                ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                Remarks = model.Remarks,

                Status = "Draft",

                Subtotal = subtotal,
                TaxAmount = taxAmount,
                GrandTotal = subtotal + taxAmount,

                OrderDetails = orderDetails
            };

            _context.PurchaseOrders.Add(purchaseOrder);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Purchase Order created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: PurchaseOrders/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .FirstOrDefaultAsync(po =>
                    po.PurchaseOrderId == id
                );

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            if (purchaseOrder.Status != "Draft")
            {
                TempData["Error"] =
                    "Only Draft Purchase Orders can be approved.";

                return RedirectToAction(nameof(Index));
            }

            purchaseOrder.Status = "Approved";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Purchase Order approved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // POST: PurchaseOrders/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .FirstOrDefaultAsync(po =>
                    po.PurchaseOrderId == id
                );

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            // Only Draft and Approved POs can be cancelled
            if (purchaseOrder.Status != "Draft" &&
                purchaseOrder.Status != "Approved")
            {
                TempData["Error"] =
                    "Only Draft or Approved Purchase Orders can be cancelled.";

                return RedirectToAction(nameof(Index));
            }

            // Do not cancel if goods have already been received
            bool hasPostedGRN = await _context.GoodsReceiptNotes
                .AnyAsync(g =>
                    g.PurchaseOrderId == id &&
                    g.Status == "Posted"
                );

            if (hasPostedGRN)
            {
                TempData["Error"] =
                    "Purchase Order cannot be cancelled because goods have already been received.";

                return RedirectToAction(nameof(Index));
            }

            purchaseOrder.Status = "Cancelled";

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Purchase Order cancelled successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Load Vendor and Status filter dropdowns
        private async Task LoadFilterDropdowns(
            int? selectedVendorId,
            string? selectedStatus)
        {
            var vendors = await _context.Vendors
                .OrderBy(v => v.CompanyName)
                .ToListAsync();

            ViewBag.Vendors = new SelectList(
                vendors,
                "VendorId",
                "CompanyName",
                selectedVendorId
            );

            var statuses = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "All Statuses"
                },
                new SelectListItem
                {
                    Value = "Draft",
                    Text = "Draft"
                },
                new SelectListItem
                {
                    Value = "Approved",
                    Text = "Approved"
                },
                new SelectListItem
                {
                    Value = "Partially Received",
                    Text = "Partially Received"
                },
                new SelectListItem
                {
                    Value = "Completed",
                    Text = "Completed"
                },
                new SelectListItem
                {
                    Value = "Cancelled",
                    Text = "Cancelled"
                }
            };

            foreach (var item in statuses)
            {
                item.Selected =
                    item.Value == (selectedStatus ?? "");
            }

            ViewBag.Statuses = statuses;
        }

        // Load active vendors and items for PO creation
        private async Task LoadDropdowns()
        {
            ViewBag.Vendors = new SelectList(
                await _context.Vendors
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.CompanyName)
                    .ToListAsync(),
                "VendorId",
                "CompanyName"
            );

            ViewBag.Items = new SelectList(
                await _context.Items
                    .Where(i => i.IsActive)
                    .OrderBy(i => i.ItemName)
                    .ToListAsync(),
                "ItemId",
                "ItemName"
            );
        }
    }
}
