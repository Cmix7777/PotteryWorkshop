using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Controllers;

public class BookingsController : Controller
{
    private readonly AppDbContext _db;

    public BookingsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Bookings/Create?serviceId=1
    public async Task<IActionResult> Create(int? serviceId)
    {
        // Получаем все услуги для выпадающего списка
        ViewBag.Services = await _db.Services.ToListAsync();
        ViewBag.SelectedServiceId = serviceId;
        
        return View();
    }

    // POST: /Bookings/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        if (ModelState.IsValid)
        {
            booking.UserId = 1; // В реальном проекте — из авторизации
            booking.Status = "новая";
            booking.CreatedAt = DateTime.Now;

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return RedirectToAction("Success");
        }

        // Если ошибка — возвращаем форму с данными
        ViewBag.Services = await _db.Services.ToListAsync();
        return View(booking);
    }

    // GET: /Bookings/Success
    public IActionResult Success()
    {
        return View();
    }
}