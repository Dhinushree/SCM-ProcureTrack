
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using DotNetAssignmentApp.Data;
using DotNetAssignmentApp.Models;
using DotNetAssignmentApp.ViewModels;
using DotNetAssignmentApp.Services;

namespace DotNetAssignmentApp.Controllers
{
    [Authorize(Policy = "WarehouseAccess")]
    public class GoodsReceiptsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly GoodsReceiptValidator _validator;

        public GoodsReceiptsController(ApplicationDbContext context)
        {
            _context = context;
            _validator = new GoodsReceiptValidator();
        }

        // GET: GoodsReceipts
        public async Task<IActionResult> Index()
        {
            var receipts = await _context.GoodsReceiptNotes
                .Include(g => g.PurchaseOrder)
                .OrderByDescending(g => g.ReceiptDate)
                .ToListAsync();

            return View(receipts);
        }

        // GET: GoodsReceipts/Create
        public async Task<IActionResult> Create()
        {
            await LoadPurchaseOrders();

            return View(new GoodsReceiptViewModel
            {
                ReceiptDate = DateTime.Today
            });
        }

        // GET: GoodsReceipts/CreateForPO
        public async Task<IActionResult> CreateForPO(int purchaseOrderId)
        {
            var order = await _context.PurchaseOrders
                .Include(p => p.OrderDetails)
                .ThenInclude(d => d.Item)
                .FirstOrDefaultAsync(p =>
                    p.PurchaseOrderId == purchaseOrderId);

            if (order == null)
                return NotFound();

            if (order.Status != "Approved" &&
                order.Status != "Partially Received")
            {
                TempData["Error"] =
                    "GRN can only be created for approved or partially received POs.";

                return RedirectToAction("Index", "PurchaseOrders");
            }

            var model = new GoodsReceiptViewModel
            {
                PurchaseOrderId = order.PurchaseOrderId,
                ReceiptDate = DateTime.Today,
                Items = new List<GoodsReceiptItemViewModel>()
            };

            foreach (var detail in order.OrderDetails)
            {
                var previouslyAccepted = await _context.GoodsReceiptDetails
                    .Where(g => g.PurchaseOrderDetailId ==
                                detail.PurchaseOrderDetailId)
                    .SumAsync(g => (decimal?)g.AcceptedQuantity) ?? 0;

                model.Items.Add(new GoodsReceiptItemViewModel
                {
                    PurchaseOrderDetailId = detail.PurchaseOrderDetailId,
                    ItemName = detail.Item?.ItemName ?? "",
                    OrderedQuantity = detail.OrderedQuantity,
                    PreviouslyAcceptedQuantity = previouslyAccepted,
                    ReceivedQuantity = 0,
                    AcceptedQuantity = 0,
                    RejectedQuantity = 0
                });
            }

            ViewBag.PONumber = order.PONumber;

            return View("Create", model);
        }

        // POST: GoodsReceipts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            GoodsReceiptViewModel model)
        {
            model.Items ??= new List<GoodsReceiptItemViewModel>();

            var order = await _context.PurchaseOrders
                .Include(p => p.OrderDetails)
                .ThenInclude(d => d.Item)
                .FirstOrDefaultAsync(p =>
                    p.PurchaseOrderId == model.PurchaseOrderId);

            if (order == null)
                return NotFound();

            if (order.Status != "Approved" &&
                order.Status != "Partially Received")
            {
                ModelState.AddModelError("",
                    "This Purchase Order cannot receive goods.");
            }

            if (!model.Items.Any())
            {
                ModelState.AddModelError("",
                    "No receipt items were provided.");
            }

            foreach (var receivedItem in model.Items)
            {
                var poDetail = order.OrderDetails.FirstOrDefault(d =>
                    d.PurchaseOrderDetailId ==
                    receivedItem.PurchaseOrderDetailId);

                if (poDetail == null)
                {
                    ModelState.AddModelError("",
                        "Invalid Purchase Order item.");

                    continue;
                }

                var previouslyAccepted = await _context.GoodsReceiptDetails
                    .Where(g => g.PurchaseOrderDetailId ==
                                poDetail.PurchaseOrderDetailId)
                    .SumAsync(g => (decimal?)g.AcceptedQuantity) ?? 0;

                // Validate received, accepted and rejected quantities
                if (!_validator.IsReceiptQuantityValid(
                    receivedItem.ReceivedQuantity,
                    receivedItem.AcceptedQuantity,
                    receivedItem.RejectedQuantity))
                {
                    ModelState.AddModelError("",
                        "Invalid receipt quantities. Accepted plus rejected must equal received quantity, and quantities cannot be negative.");
                }

                // Validate cumulative accepted quantity
                if (!_validator.IsAcceptedQuantityWithinOrder(
                    poDetail.OrderedQuantity,
                    previouslyAccepted,
                    receivedItem.AcceptedQuantity))
                {
                    ModelState.AddModelError("",
                        $"Accepted quantity exceeds ordered quantity for {poDetail.Item?.ItemName}.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PONumber = order.PONumber;
                return View(model);
            }

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var grn = new GoodsReceiptNote
                {
                    GRNNumber = "GRN-" +
                        DateTime.Now.ToString("yyyyMMddHHmmssfff"),

                    PurchaseOrderId = order.PurchaseOrderId,
                    ReceiptDate = model.ReceiptDate,
                    Remarks = model.Remarks,
                    Status = "Posted"
                };

                _context.GoodsReceiptNotes.Add(grn);

                await _context.SaveChangesAsync();

                foreach (var receivedItem in model.Items)
                {
                    if (receivedItem.ReceivedQuantity == 0)
                        continue;

                    var poDetail = order.OrderDetails.First(d =>
                        d.PurchaseOrderDetailId ==
                        receivedItem.PurchaseOrderDetailId);

                    var item = await _context.Items.FindAsync(poDetail.ItemId);

                    if (item == null)
                    {
                        throw new InvalidOperationException(
                            "Item not found.");
                    }

                    var grnDetail = new GoodsReceiptDetail
                    {
                        GoodsReceiptNoteId = grn.GoodsReceiptNoteId,

                        PurchaseOrderDetailId =
                            poDetail.PurchaseOrderDetailId,

                        ReceivedQuantity =
                            receivedItem.ReceivedQuantity,

                        AcceptedQuantity =
                            receivedItem.AcceptedQuantity,

                        RejectedQuantity =
                            receivedItem.RejectedQuantity,

                        InspectionRemarks =
                            receivedItem.InspectionRemarks
                    };

                    _context.GoodsReceiptDetails.Add(grnDetail);

                    // Update item stock using accepted quantity only
                    item.CurrentStock += receivedItem.AcceptedQuantity;

                    await _context.SaveChangesAsync();

                    // Create inventory ledger entry
                    if (receivedItem.AcceptedQuantity > 0)
                    {
                        _context.InventoryLedgers.Add(
                            new InventoryLedger
                            {
                                ItemId = item.ItemId,

                                GoodsReceiptDetailId =
                                    grnDetail.GoodsReceiptDetailId,

                                TransactionType = "IN",

                                Quantity =
                                    receivedItem.AcceptedQuantity,

                                BalanceAfter = item.CurrentStock,

                                ReferenceNumber = grn.GRNNumber,

                                TransactionDate = DateTime.UtcNow,

                                Remarks = "Goods received against " +
                                    order.PONumber
                            });
                    }
                }

                await _context.SaveChangesAsync();

                // Update Purchase Order status
                bool allCompleted = true;
                bool anyReceived = false;

                foreach (var detail in order.OrderDetails)
                {
                    var totalAccepted = await _context.GoodsReceiptDetails
                        .Where(g => g.PurchaseOrderDetailId ==
                                    detail.PurchaseOrderDetailId)
                        .SumAsync(g => (decimal?)g.AcceptedQuantity) ?? 0;

                    if (totalAccepted > 0)
                        anyReceived = true;

                    if (totalAccepted < detail.OrderedQuantity)
                        allCompleted = false;
                }

                order.Status = allCompleted
                    ? "Completed"
                    : anyReceived
                        ? "Partially Received"
                        : "Approved";

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["Success"] =
                    "Goods Receipt Note created and stock updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError("",
                    "Unable to save GRN. Please try again.");

                ViewBag.PONumber = order.PONumber;

                return View(model);
            }
        }

        // Load active Purchase Orders
        private async Task LoadPurchaseOrders()
        {
            var orders = await _context.PurchaseOrders
                .Where(p =>
                    p.Status == "Approved" ||
                    p.Status == "Partially Received")
                .OrderByDescending(p => p.OrderDate)
                .ToListAsync();

            ViewBag.PurchaseOrders = new SelectList(
                orders,
                "PurchaseOrderId",
                "PONumber"
            );
        }
    }
}
