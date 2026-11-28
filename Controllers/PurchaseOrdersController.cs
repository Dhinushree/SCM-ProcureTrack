
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
        public async Task<IActionResult> Index(
            int? vendorId,
            DateTime? fromDate,
            DateTime? toDate,
            string? status)
        {
            var query = _context.PurchaseOrders
                .Include(po => po.Vendor)
                .AsQueryable();

            if (vendorId.HasValue && vendorId.Value > 0)
            {
                query = query.Where(po =>
                    po.VendorId == vendorId.Value);
            }

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
                query = query.Where(po => po.Status == status);
            }

            var orders = await query
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();

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

            ValidatePurchaseOrder(model);

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

        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.OrderDetails)
                .FirstOrDefaultAsync(po =>
                    po.PurchaseOrderId == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            if (purchaseOrder.Status != "Draft")
            {
                TempData["Error"] =
                    "Only Draft Purchase Orders can be edited.";

                return RedirectToAction(nameof(Index));
            }

            bool hasPostedGRN = await _context.GoodsReceiptNotes
                .AnyAsync(grn =>
                    grn.PurchaseOrderId == id &&
                    grn.Status == "Posted");

            if (hasPostedGRN)
            {
                TempData["Error"] =
                    "Purchase Order cannot be edited because a GRN has already been posted.";

                return RedirectToAction(nameof(Index));
            }

            var model = new PurchaseOrderViewModel
            {
                VendorId = purchaseOrder.VendorId,
                OrderDate = purchaseOrder.OrderDate,
                ExpectedDeliveryDate =
                    purchaseOrder.ExpectedDeliveryDate,
                Remarks = purchaseOrder.Remarks,

                Items = purchaseOrder.OrderDetails
                    .Select(detail => new PurchaseOrderItemViewModel
                    {
                        ItemId = detail.ItemId,
                        OrderedQuantity = detail.OrderedQuantity,
                        UnitPrice = detail.UnitPrice,
                        TaxPercentage = detail.TaxPercentage
                    })
                    .ToList()
            };

            await LoadEditDropdowns();

            ViewBag.PurchaseOrderId = id;
            ViewBag.PONumber = purchaseOrder.PONumber;

            return View(model);
        }

        // POST: PurchaseOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            PurchaseOrderViewModel model)
        {
            model.Items ??= new List<PurchaseOrderItemViewModel>();

            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.OrderDetails)
                .FirstOrDefaultAsync(po =>
                    po.PurchaseOrderId == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            if (purchaseOrder.Status != "Draft")
            {
                TempData["Error"] =
                    "Only Draft Purchase Orders can be edited.";

                return RedirectToAction(nameof(Index));
            }

            bool hasPostedGRN = await _context.GoodsReceiptNotes
                .AnyAsync(grn =>
                    grn.PurchaseOrderId == id &&
                    grn.Status == "Posted");

            if (hasPostedGRN)
            {
                TempData["Error"] =
                    "Purchase Order cannot be edited because a GRN has already been posted.";

                return RedirectToAction(nameof(Index));
            }

            ValidatePurchaseOrder(model);

            if (!ModelState.IsValid)
            {
                await LoadEditDropdowns();

                ViewBag.PurchaseOrderId = id;
                ViewBag.PONumber = purchaseOrder.PONumber;

                return View(model);
            }

            // Validate active vendor
            bool vendorExists = await _context.Vendors
                .AnyAsync(v =>
                    v.VendorId == model.VendorId &&
                    v.IsActive);

            if (!vendorExists)
            {
                ModelState.AddModelError(
                    nameof(model.VendorId),
                    "Select a valid active vendor.");
            }

            // Validate active items
            var itemIds = model.Items
                .Select(x => x.ItemId)
                .Distinct()
                .ToList();

            int validItemCount = await _context.Items
                .CountAsync(item =>
                    itemIds.Contains(item.ItemId) &&
                    item.IsActive);

            if (validItemCount != itemIds.Count)
            {
                ModelState.AddModelError(
                    "",
                    "Select valid active items.");
            }

            if (!ModelState.IsValid)
            {
                await LoadEditDropdowns();

                ViewBag.PurchaseOrderId = id;
                ViewBag.PONumber = purchaseOrder.PONumber;

                return View(model);
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                purchaseOrder.VendorId = model.VendorId;
                purchaseOrder.OrderDate = model.OrderDate;
                purchaseOrder.ExpectedDeliveryDate =
                    model.ExpectedDeliveryDate;
                purchaseOrder.Remarks = model.Remarks;

                // Remove existing line items
                _context.PurchaseOrderDetails.RemoveRange(
                    purchaseOrder.OrderDetails);

                decimal subtotal = 0;
                decimal taxAmount = 0;

                var updatedDetails =
                    new List<PurchaseOrderDetail>();

                foreach (var item in model.Items)
                {
                    decimal amount =
                        item.OrderedQuantity * item.UnitPrice;

                    decimal tax =
                        amount * item.TaxPercentage / 100;

                    subtotal += amount;
                    taxAmount += tax;

                    updatedDetails.Add(new PurchaseOrderDetail
                    {
                        PurchaseOrderId =
                            purchaseOrder.PurchaseOrderId,

                        ItemId = item.ItemId,
                        OrderedQuantity = item.OrderedQuantity,
                        UnitPrice = item.UnitPrice,
                        TaxPercentage = item.TaxPercentage,
                        Amount = amount + tax
                    });
                }

                purchaseOrder.OrderDetails = updatedDetails;

                purchaseOrder.Subtotal = subtotal;
                purchaseOrder.TaxAmount = taxAmount;
                purchaseOrder.GrandTotal = subtotal + taxAmount;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] =
                    "Purchase Order updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    "",
                    "An error occurred while updating the Purchase Order. Please try again.");

                await LoadEditDropdowns();

                ViewBag.PurchaseOrderId = id;
                ViewBag.PONumber = purchaseOrder.PONumber;

                return View(model);
            }
        }

        // POST: PurchaseOrders/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .FirstOrDefaultAsync(po =>
                    po.PurchaseOrderId == id);

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
                    po.PurchaseOrderId == id);

            if (purchaseOrder == null)
            {
                return NotFound();
            }

            if (purchaseOrder.Status != "Draft" &&
                purchaseOrder.Status != "Approved")
            {
                TempData["Error"] =
                    "Only Draft or Approved Purchase Orders can be cancelled.";

                return RedirectToAction(nameof(Index));
            }

            bool hasPostedGRN = await _context.GoodsReceiptNotes
                .AnyAsync(g =>
                    g.PurchaseOrderId == id &&
                    g.Status == "Posted");

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

        // Common Purchase Order validation
        private void ValidatePurchaseOrder(
            PurchaseOrderViewModel model)
        {
            if (!model.Items.Any())
            {
                ModelState.AddModelError(
                    "",
                    "Add at least one item to the Purchase Order.");
            }

            if (model.ExpectedDeliveryDate.Date <
                model.OrderDate.Date)
            {
                ModelState.AddModelError(
                    nameof(model.ExpectedDeliveryDate),
                    "Expected delivery date cannot be before order date.");
            }

            if (model.Items.Any(item => item.ItemId <= 0))
            {
                ModelState.AddModelError(
                    "",
                    "Select a valid item for every row.");
            }
        }

        // Filter dropdowns
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
                selectedVendorId);

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

        // Dropdowns for Create
        private async Task LoadDropdowns()
        {
            ViewBag.Vendors = new SelectList(
                await _context.Vendors
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.CompanyName)
                    .ToListAsync(),
                "VendorId",
                "CompanyName");

            ViewBag.Items = new SelectList(
                await _context.Items
                    .Where(i => i.IsActive)
                    .OrderBy(i => i.ItemName)
                    .ToListAsync(),
                "ItemId",
                "ItemName");
        }

        // Dropdowns for Edit
        private async Task LoadEditDropdowns()
        {
            ViewBag.Vendors = new SelectList(
                await _context.Vendors
                    .Where(v => v.IsActive)
                    .OrderBy(v => v.CompanyName)
                    .ToListAsync(),
                "VendorId",
                "CompanyName");

            ViewBag.Items = await _context.Items
                .Where(i => i.IsActive)
                .OrderBy(i => i.ItemName)
                .Select(i => new SelectListItem
                {
                    Value = i.ItemId.ToString(),
                    Text = i.ItemName
                })
                .ToListAsync();
        }
    }
}
