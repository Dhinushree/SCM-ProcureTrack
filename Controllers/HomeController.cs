using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using DotNetAssignmentApp.Models;
using DotNetAssignmentApp.Data;

namespace DotNetAssignmentApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalVendors =
            await _context.Vendors.CountAsync();

        ViewBag.TotalPurchaseOrders =
            await _context.PurchaseOrders.CountAsync();

        ViewBag.OpenPurchaseOrders =
            await _context.PurchaseOrders
                .CountAsync(x => x.Status == "Approved"
                              || x.Status == "Partially Received");

        // Calculate pending delivery value using remaining quantities
        var openOrders = await _context.PurchaseOrders
            .Where(x => x.Status == "Approved"
                     || x.Status == "Partially Received")
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Item)
            .ToListAsync();

        decimal pendingDeliveryValue = 0;

        foreach (var order in openOrders)
        {
            foreach (var detail in order.OrderDetails)
            {
                var acceptedQuantity = await _context.GoodsReceiptDetails
                    .Where(x => x.PurchaseOrderDetailId ==
                                detail.PurchaseOrderDetailId)
                    .SumAsync(x => (decimal?)x.AcceptedQuantity) ?? 0;

                var pendingQuantity =
                    detail.OrderedQuantity - acceptedQuantity;

                if (pendingQuantity > 0)
                {
                    var pendingSubtotal =
                        pendingQuantity * detail.UnitPrice;

                    var pendingTax =
                        pendingSubtotal * detail.TaxPercentage / 100;

                    pendingDeliveryValue +=
                        pendingSubtotal + pendingTax;
                }
            }
        }

        ViewBag.PendingDeliveryValue = pendingDeliveryValue;

        ViewBag.LowStockItems =
            await _context.Items
                .CountAsync(x => x.CurrentStock <= x.ReorderLevel);

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ??
                        HttpContext.TraceIdentifier
        });
    }
}