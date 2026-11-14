using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;
using PotteryWorkshop.ViewModels;

namespace PotteryWorkshop.Controllers;

/// Главная страница и страница команды
public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;

        var model = new HomeViewModel
        {
            Services = await _db.Services
                .OrderBy(s => s.Id)
                .Take(3)
                .ToListAsync(),

            // Свободные места
            Events = await _db.Events
                .Where(e => e.StartsAt > now)
                .OrderBy(e => e.StartsAt)
                .Take(3)
                .Select(e => new EventCardViewModel
                {
                    Event = e,
                    FreePlaces = e.PlacesTotal - e.Registrations.Count
                })
                .ToListAsync(),

            Products = await _db.Products
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.Stock > 0)
                .OrderBy(p => p.Id)
                .Take(4)
                .ToListAsync(),

            // отзывы
            Reviews = await _db.Reviews
                .Include(r => r.User)
                .Where(r => r.Status == ReviewStatus.Published)
                .OrderByDescending(r => r.CreatedAt)
                .Take(3)
                .ToListAsync()
        };

        return View(model);
    }

    /// О мастерской
    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contacts()
    {
        return View(new FeedbackViewModel());
    }

    /// Сохраняет сообщение формы
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contacts(FeedbackViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _db.FeedbackMessages.Add(new FeedbackMessage
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Topic = model.Topic,
            Message = model.Message.Trim()
        });
        await _db.SaveChangesAsync();

        TempData["Message"] = "Спасибо! Сообщение отправлено, мы ответим в течение дня.";
        return RedirectToAction(nameof(Contacts));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    /// распределение задач в команде
    public IActionResult Team()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
