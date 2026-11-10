using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Logic;

namespace PotteryWorkshop.Controllers;

/// <summary>Календарь мероприятий и регистрация на них.</summary>
public class EventsController : BaseController
{
    private readonly AppDbContext _db;
    private readonly EventManager _events;

    public EventsController(AppDbContext db, EventManager events)
    {
        _db = db;
        _events = events;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;
        var events = await _db.Events
            .Include(e => e.Registrations)
            .Where(e => e.StartsAt > now)
            .OrderBy(e => e.StartsAt)
            .ToListAsync();

        // Для вошедшего пользователя отмечаем, куда он уже зарегистрирован
        ViewBag.MyEventIds = User.Identity?.IsAuthenticated == true
            ? events.Where(e => e.Registrations.Any(r => r.UserId == CurrentUserId)).Select(e => e.Id).ToList()
            : new List<int>();

        return View(events);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int id)
    {
        var result = await _events.RegisterAsync(CurrentUserId, id);
        TempData[result.Success ? "Message" : "Error"] = result.Success ? "Вы зарегистрированы на мероприятие" : result.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unregister(int id)
    {
        var result = await _events.UnregisterAsync(CurrentUserId, id);
        TempData[result.Success ? "Message" : "Error"] = result.Success ? "Регистрация отменена" : result.Error;
        return RedirectToAction(nameof(Index));
    }
}
