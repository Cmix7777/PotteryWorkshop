using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Logic;

namespace PotteryWorkshop.Controllers;

/// <summary>Запись на занятия: расписание, запись, мои записи, отмена.</summary>
public class BookingController : BaseController
{
    private readonly AppDbContext _db;
    private readonly BookingManager _manager;

    public BookingController(AppDbContext db, BookingManager manager)
    {
        _db = db;
        _manager = manager;
    }

    /// <summary>Расписание ближайших занятий. Можно отфильтровать по услуге.</summary>
    public async Task<IActionResult> Index(int? serviceId)
    {
        var now = DateTime.Now;

        var query = _db.Lessons
            .Include(l => l.Service)
            .Include(l => l.Bookings)
            .Where(l => l.StartsAt > now);

        if (serviceId.HasValue)
        {
            query = query.Where(l => l.ServiceId == serviceId.Value);
        }

        var lessons = await query.OrderBy(l => l.StartsAt).ToListAsync();

        ViewBag.Services = await _db.Services.OrderBy(s => s.Id).ToListAsync();
        ViewBag.ServiceId = serviceId;

        return View(lessons);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int lessonId)
    {
        var result = await _manager.BookAsync(CurrentUserId, lessonId);

        if (result.Success)
        {
            TempData["Message"] = "Вы записаны. Администратор подтвердит запись, статус можно посмотреть в разделе «Мои записи».";
            return RedirectToAction(nameof(My));
        }

        TempData["Error"] = result.Error;
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Записи текущего пользователя, новые сверху.</summary>
    [Authorize]
    public async Task<IActionResult> My()
    {
        var userId = CurrentUserId;

        var bookings = await _db.Bookings
            .Include(b => b.Lesson)
                .ThenInclude(l => l.Service)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Lesson.StartsAt)
            .ToListAsync();

        return View(bookings);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _manager.CancelAsync(CurrentUserId, id);

        if (result.Success)
        {
            TempData["Message"] = "Запись отменена";
        }
        else
        {
            TempData["Error"] = result.Error;
        }

        return RedirectToAction(nameof(My));
    }
}
