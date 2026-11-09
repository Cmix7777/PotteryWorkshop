using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;

namespace PotteryWorkshop.Controllers;

public class ServicesController : Controller
{
    private readonly AppDbContext _db;

    public ServicesController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Services
    public async Task<IActionResult> Index()
    {
        var services = await _db.Services.ToListAsync();
        return View(services);
    }
}