using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Controllers;

public class ProductsController : Controller
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /Products
    // Показывает все товары, можно фильтровать по категории
    public async Task<IActionResult> Index(int? categoryId)
    {
        var products = _db.Products.Include(p => p.Category).AsQueryable();

        // Если выбрана категория, фильтруем
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value);
        }

        // Получаем все категории для фильтра в sidebar
        ViewBag.Categories = await _db.Categories.ToListAsync();
        ViewBag.CurrentCategoryId = categoryId;

        return View(await products.ToListAsync());
    }

    // GET: /Products/Details/5
    // Страница конкретного товара
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