using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Logic;
using PotteryWorkshop.Models;
using PotteryWorkshop.ViewModels;

namespace PotteryWorkshop.Controllers;

/// <summary>Отзывы: список опубликованных с фильтром по услуге и форма нового отзыва.</summary>
public class ReviewsController : BaseController
{
    private readonly AppDbContext _db;
    private readonly ReviewManager _reviews;

    public ReviewsController(AppDbContext db, ReviewManager reviews)
    {
        _db = db;
        _reviews = reviews;
    }

    public async Task<IActionResult> Index(string? filter)
    {
        // Только отзывы, прошедшие модерацию
        var query = _db.Reviews
            .Include(r => r.User)
            .Where(r => r.Status == ReviewStatus.Published);

        // Список фильтров собираем до фильтрации, чтобы кнопки не пропадали
        ViewBag.Filters = await query
            .Where(r => r.ServiceType != "")
            .Select(r => r.ServiceType)
            .Distinct()
            .ToListAsync();
        ViewBag.CurrentFilter = filter;

        if (!string.IsNullOrEmpty(filter))
        {
            query = query.Where(r => r.ServiceType == filter);
        }

        return View(await query.OrderByDescending(r => r.CreatedAt).ToListAsync());
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        ViewBag.CanReview = await _reviews.CanLeaveReviewAsync(CurrentUserId);
        return View(new ReviewFormViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CanReview = true;
            return View(model);
        }

        var result = await _reviews.AddAsync(CurrentUserId, model.ServiceType, model.Rating, model.Text);

        if (!result.Success)
        {
            ViewBag.CanReview = false;
            ModelState.AddModelError(string.Empty, result.Error!);
            return View(model);
        }

        return RedirectToAction(nameof(Success));
    }

    public IActionResult Success()
    {
        return View();
    }
}
