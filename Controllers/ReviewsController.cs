using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PotteryWorkshop.Controllers;

public class ReviewsController : Controller
{
    private readonly AppDbContext _db;

    public ReviewsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Reviews
    // Показывает список опубликованных отзывов с возможностью фильтрации
    public async Task<IActionResult> Index(string? filter)
    {
        // Базовый запрос: берем только одобренные отзывы
        var query = _db.Reviews.Where(r => r.Status == "опубликован");

        // Если выбран фильтр, добавляем условие
        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(r => r.ServiceType == filter);
        }

        // Сортировка: новые отзывы сверху
        var reviews = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();

        // Собираем список уникальных типов услуг для sidebar (только непустые)
        var filters = reviews
            .Select(r => r.ServiceType)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct()
            .ToList();

        ViewBag.Filters = filters;
        ViewBag.CurrentFilter = filter;

        return View(reviews);
    }

    // GET: /Reviews/Create
    // Страница с формой добавления отзыва
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Reviews/Create
    // Обработка формы после нажатия кнопки "Отправить"
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Review review)
    {
        if (ModelState.IsValid)
        {
            // В реальном проекте UserId берется из авторизованного пользователя
            review.UserId = 1; 
            
            // Новые отзывы сначала попадают на модерацию
            review.Status = "на модерации";
            review.CreatedAt = DateTime.Now;

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            // Перенаправляем на страницу успеха
            return RedirectToAction("Success");
        }

        // Если есть ошибки валидации, возвращаем форму с введенными данными
        return View(review);
    }

    // GET: /Reviews/Success
    // Страница благодарности после отправки отзыва
    public IActionResult Success()
    {
        return View();
    }
}