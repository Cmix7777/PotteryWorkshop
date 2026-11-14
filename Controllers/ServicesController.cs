using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;

namespace PotteryWorkshop.Controllers;

/// Услуги
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

        var services = await _db.Services
            .Include(s => s.Lessons.Where(l => l.StartsAt > now).OrderBy(l => l.StartsAt))
                .ThenInclude(l => l.Bookings)
            .OrderBy(s => s.Id)
            .ToListAsync();

        return View(services);
    }
}
