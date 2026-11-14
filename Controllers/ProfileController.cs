using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.ViewModels;

namespace PotteryWorkshop.Controllers;

/// Личный кабинет
[Authorize]
public class ProfileController : BaseController
{
    private readonly AppDbContext _db;

    public ProfileController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var userId = CurrentUserId;
        var now = DateTime.Now;
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        ViewBag.Orders = await _db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        ViewBag.Events = await _db.EventRegistrations
            .Include(r => r.Event)
            .Where(r => r.UserId == userId && r.Event.StartsAt > now)
            .OrderBy(r => r.Event.StartsAt)
            .ToListAsync();

        return View(new ProfileViewModel { FullName = user.FullName, Phone = user.Phone, Email = user.Email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        var user = await _db.Users.FindAsync(CurrentUserId);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Проверьте имя и телефон";
            return RedirectToAction(nameof(Index));
        }

        user.FullName = model.FullName.Trim();
        user.Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
        await _db.SaveChangesAsync();

        TempData["Message"] = "Данные сохранены. Новое имя появится в шапке после повторного входа.";
        return RedirectToAction(nameof(Index));
    }
}
