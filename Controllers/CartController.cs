using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Logic;
using PotteryWorkshop.ViewModels;

namespace PotteryWorkshop.Controllers;

/// Корзина и оформление заказа
[Authorize]
public class CartController : BaseController
{
    private readonly AppDbContext _db;
    private readonly OrderManager _orders;

    public CartController(AppDbContext db, OrderManager orders)
    {
        _db = db;
        _orders = orders;
    }

    public async Task<IActionResult> Index()
    {
        var userId = CurrentUserId;
        var items = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Id)
            .ToListAsync();

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        var result = await _orders.AddToCartAsync(CurrentUserId, productId, quantity);

        if (!result.Success)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        TempData["Message"] = "Товар добавлен в корзину";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, int quantity)
    {
        var result = await _orders.UpdateQuantityAsync(CurrentUserId, id, quantity);
        if (!result.Success)
        {
            TempData["Error"] = result.Error;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = CurrentUserId;
        if (!await _db.CartItems.AnyAsync(c => c.UserId == userId))
        {
            return RedirectToAction(nameof(Index));
        }

        // Телефон из профиля
        var user = await _db.Users.FindAsync(userId);
        return View(new CheckoutViewModel { Phone = user?.Phone ?? "" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _orders.PlaceOrderAsync(CurrentUserId, model.Phone, model.Comment);

        if (!result.Success)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        TempData["Message"] = "Заказ оформлен. Мы позвоним, когда он будет готов к выдаче.";
        return RedirectToAction("Index", "Profile");
    }
}
