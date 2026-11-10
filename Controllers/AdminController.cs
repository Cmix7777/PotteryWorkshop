using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;
using PotteryWorkshop.ViewModels;

namespace PotteryWorkshop.Controllers;

/// <summary>
/// Админ-панель. Модератор работает с расписанием, мероприятиями и записями и видит отзывы и сообщения.
/// Товары, заказы, пользователи, модерация отзывов и статистика доступны только администратору.
/// </summary>
[Authorize(Roles = "Admin,Moderator")]
public class AdminController : BaseController
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Главная админки: короткая сводка, статистика для администратора.</summary>
    public async Task<IActionResult> Index()
    {
        var now = DateTime.Now;
        var model = new AdminStatsViewModel
        {
            UsersCount = await _db.Users.CountAsync(),
            NewBookings = await _db.Bookings.CountAsync(b => b.Status == BookingStatus.New),
            UpcomingLessons = await _db.Lessons.CountAsync(l => l.StartsAt > now),
            NewOrders = await _db.Orders.CountAsync(o => o.Status == OrderStatus.New),
            Revenue = await _db.Orders.Where(o => o.Status == OrderStatus.Issued).SumAsync(o => o.Total),
            ReviewsOnModeration = await _db.Reviews.CountAsync(r => r.Status == ReviewStatus.Moderation),
            NewMessages = await _db.FeedbackMessages.CountAsync(),
            LowStock = await _db.Products.Where(p => p.Stock <= 2).OrderBy(p => p.Stock).ToListAsync()
        };

        // Самые популярные услуги по числу неотменённых записей
        var popular = await _db.Bookings
            .Where(b => b.Status != BookingStatus.Cancelled)
            .GroupBy(b => b.Lesson.Service.Title)
            .Select(g => new { Service = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
        model.PopularServices = popular.Select(x => (x.Service, x.Count)).ToList();

        return View(model);
    }

    // ---------- Записи на занятия ----------

    public async Task<IActionResult> Bookings()
    {
        var bookings = await _db.Bookings
            .Include(b => b.User)
            .Include(b => b.Lesson)
                .ThenInclude(l => l.Service)
            .OrderByDescending(b => b.Lesson.StartsAt)
            .ToListAsync();

        return View(bookings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetBookingStatus(int id, BookingStatus status)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking != null)
        {
            booking.Status = status;
            await _db.SaveChangesAsync();
            TempData["Message"] = "Статус записи изменён";
        }
        return RedirectToAction(nameof(Bookings));
    }

    // ---------- Расписание ----------

    public async Task<IActionResult> Lessons()
    {
        await FillServicesAsync();
        ViewBag.Lessons = await _db.Lessons
            .Include(l => l.Service)
            .Include(l => l.Bookings)
            .Where(l => l.StartsAt > DateTime.Today)
            .OrderBy(l => l.StartsAt)
            .ToListAsync();

        return View(new LessonFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lessons(LessonFormViewModel model)
    {
        if (model.StartsAt <= DateTime.Now)
        {
            ModelState.AddModelError(nameof(model.StartsAt), "Занятие должно быть в будущем");
        }

        if (!ModelState.IsValid)
        {
            await FillServicesAsync();
            ViewBag.Lessons = await _db.Lessons.Include(l => l.Service).Include(l => l.Bookings)
                .Where(l => l.StartsAt > DateTime.Today).OrderBy(l => l.StartsAt).ToListAsync();
            return View(model);
        }

        _db.Lessons.Add(new Lesson
        {
            ServiceId = model.ServiceId,
            StartsAt = model.StartsAt,
            PlacesTotal = model.PlacesTotal,
            Teacher = model.Teacher.Trim()
        });
        await _db.SaveChangesAsync();

        TempData["Message"] = "Занятие добавлено в расписание";
        return RedirectToAction(nameof(Lessons));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        var lesson = await _db.Lessons.Include(l => l.Bookings).FirstOrDefaultAsync(l => l.Id == id);
        if (lesson == null)
        {
            return RedirectToAction(nameof(Lessons));
        }

        // Занятие с активными записями удалять нельзя, сначала нужно отменить записи
        if (lesson.Bookings.Any(b => b.Status != BookingStatus.Cancelled))
        {
            TempData["Error"] = "На занятие есть записи. Сначала отмените их в разделе «Записи».";
            return RedirectToAction(nameof(Lessons));
        }

        _db.Lessons.Remove(lesson);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Занятие удалено";
        return RedirectToAction(nameof(Lessons));
    }

    // ---------- Мероприятия ----------

    public async Task<IActionResult> Events()
    {
        ViewBag.Events = await LoadEventsAsync();
        return View(new EventFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Events(EventFormViewModel model)
    {
        if (model.StartsAt <= DateTime.Now)
        {
            ModelState.AddModelError(nameof(model.StartsAt), "Мероприятие должно быть в будущем");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Events = await LoadEventsAsync();
            return View(model);
        }

        _db.Events.Add(new Event
        {
            Title = model.Title.Trim(),
            Description = model.Description?.Trim() ?? "",
            StartsAt = model.StartsAt,
            Place = model.Place?.Trim() ?? "",
            PlacesTotal = model.PlacesTotal,
            Price = model.Price
        });
        await _db.SaveChangesAsync();

        TempData["Message"] = "Мероприятие добавлено";
        return RedirectToAction(nameof(Events));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev != null)
        {
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();
            TempData["Message"] = "Мероприятие удалено";
        }
        return RedirectToAction(nameof(Events));
    }

    // ---------- Отзывы ----------

    public async Task<IActionResult> Reviews()
    {
        var reviews = await _db.Reviews
            .Include(r => r.User)
            .OrderBy(r => r.Status)
            .ThenByDescending(r => r.CreatedAt)
            .ToListAsync();

        return View(reviews);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetReviewStatus(int id, ReviewStatus status)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review != null)
        {
            review.Status = status;
            await _db.SaveChangesAsync();
            TempData["Message"] = "Статус отзыва изменён";
        }
        return RedirectToAction(nameof(Reviews));
    }

    // ---------- Сообщения ----------

    public async Task<IActionResult> Messages()
    {
        return View(await _db.FeedbackMessages.OrderByDescending(m => m.CreatedAt).ToListAsync());
    }

    // ---------- Товары (только администратор) ----------

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Products()
    {
        var products = await _db.Products.Include(p => p.Category).OrderBy(p => p.Title).ToListAsync();
        return View(products);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditProduct(int? id)
    {
        await FillCategoriesAsync();

        if (id == null)
        {
            return View(new ProductFormViewModel());
        }

        var p = await _db.Products.FindAsync(id);
        if (p == null)
        {
            return NotFound();
        }

        return View(new ProductFormViewModel
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            Title = p.Title,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            IsFeatured = p.IsFeatured
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await FillCategoriesAsync();
            return View(model);
        }

        Product? product;
        if (model.Id == 0)
        {
            product = new Product();
            _db.Products.Add(product);
        }
        else
        {
            product = await _db.Products.FindAsync(model.Id);
            if (product == null)
            {
                return NotFound();
            }
        }

        product.CategoryId = model.CategoryId;
        product.Title = model.Title.Trim();
        product.Description = model.Description?.Trim() ?? "";
        product.Price = model.Price;
        product.Stock = model.Stock;
        product.IsFeatured = model.IsFeatured;
        await _db.SaveChangesAsync();

        TempData["Message"] = "Товар сохранён";
        return RedirectToAction(nameof(Products));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
        {
            return RedirectToAction(nameof(Products));
        }

        // Товар из оформленных заказов не удаляем, чтобы не испортить историю покупок
        if (await _db.OrderItems.AnyAsync(i => i.ProductId == id))
        {
            TempData["Error"] = "Товар есть в заказах, удалить его нельзя. Можно поставить остаток 0.";
            return RedirectToAction(nameof(Products));
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        TempData["Message"] = "Товар удалён";
        return RedirectToAction(nameof(Products));
    }

    // ---------- Заказы (только администратор) ----------

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Orders()
    {
        var orders = await _db.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return View(orders);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetOrderStatus(int id, OrderStatus status)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order != null && order.Status != status)
        {
            // При отмене возвращаем товары на склад
            if (status == OrderStatus.Cancelled && order.Status != OrderStatus.Cancelled)
            {
                foreach (var item in order.Items)
                {
                    item.Product.Stock += item.Quantity;
                }
            }

            order.Status = status;
            await _db.SaveChangesAsync();
            TempData["Message"] = "Статус заказа изменён";
        }
        return RedirectToAction(nameof(Orders));
    }

    // ---------- Пользователи и роли (только администратор) ----------

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Users()
    {
        return View(await _db.Users.OrderBy(u => u.Role).ThenBy(u => u.FullName).ToListAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRole(int id, UserRole role)
    {
        if (id == CurrentUserId)
        {
            TempData["Error"] = "Нельзя изменить роль самому себе";
            return RedirectToAction(nameof(Users));
        }

        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            user.Role = role;
            await _db.SaveChangesAsync();
            TempData["Message"] = "Роль изменена. Она применится после повторного входа пользователя.";
        }
        return RedirectToAction(nameof(Users));
    }

    // ---------- Вспомогательные методы ----------

    private async Task FillServicesAsync()
    {
        ViewBag.ServiceList = await _db.Services.OrderBy(s => s.Id).ToListAsync();
    }

    private async Task FillCategoriesAsync()
    {
        ViewBag.CategoryList = await _db.Categories.OrderBy(c => c.Id).ToListAsync();
    }

    private Task<List<Event>> LoadEventsAsync()
    {
        return _db.Events
            .Include(e => e.Registrations)
            .Where(e => e.StartsAt > DateTime.Today)
            .OrderBy(e => e.StartsAt)
            .ToListAsync();
    }
}
