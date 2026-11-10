using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;

namespace PotteryWorkshop.Controllers;

/// <summary>Страница «Услуги»: все направления и ближайшие занятия по каждому.</summary>
public class ServicesController : Controller
{
    private readonly AppDbContext _db;

    public ServicesController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;

        // Для каждой услуги подгружаем только будущие занятия и записи на них (чтобы посчитать места)
        var services = await _db.Services
            .Include(s => s.Lessons.Where(l => l.StartsAt > now).OrderBy(l => l.StartsAt))
                .ThenInclude(l => l.Bookings)
            .OrderBy(s => s.Id)
            .ToListAsync();

        return View(services);
    }
}
