using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;

namespace PotteryWorkshop.Controllers;

/// <summary>Магазин: каталог с фильтром по категориям и поиском, карточка товара.</summary>
public class ProductsController : Controller
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int? categoryId, string? search)
    {
        var products = _db.Products.Include(p => p.Category).AsQueryable();

        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        // Поиск по названию и описанию без учёта регистра
        if (!string.IsNullOrWhiteSpace(search))
        {
            var text = search.Trim().ToLower();
            products = products.Where(p => p.Title.ToLower().Contains(text) || p.Description.ToLower().Contains(text));
        }

        ViewBag.Categories = await _db.Categories.OrderBy(c => c.Id).ToListAsync();
        ViewBag.CurrentCategoryId = categoryId;
        ViewBag.Search = search;

        return View(await products.OrderBy(p => p.Title).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}
