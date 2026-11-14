using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Logic;

/// Модуль корзины и заказов: добавление в корзину и оформление заказа со всеми проверками
public class OrderManager
{
    private readonly AppDbContext _db;

    public OrderManager(AppDbContext db)
    {
        _db = db;
    }

    /// Добавляет товар в корзину. Нельзя положить больше, чем есть на складе
    public async Task<OperationResult> AddToCartAsync(int userId, int productId, int quantity)
    {
        if (quantity < 1)
        {
            return OperationResult.Fail("Количество должно быть не меньше 1");
        }

        var product = await _db.Products.FindAsync(productId);
        if (product == null)
        {
            return OperationResult.Fail("Товар не найден");
        }

        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        var newQuantity = (item?.Quantity ?? 0) + quantity;

        if (newQuantity > product.Stock)
        {
            return OperationResult.Fail($"На складе осталось только {product.Stock} шт.");
        }

        if (item == null)
        {
            _db.CartItems.Add(new CartItem { UserId = userId, ProductId = productId, Quantity = quantity });
        }
        else
        {
            item.Quantity = newQuantity;
        }

        await _db.SaveChangesAsync();
        return OperationResult.Ok();
    }

    /// Меняет количество товара в корзине. Ноль удаляет строку
    public async Task<OperationResult> UpdateQuantityAsync(int userId, int cartItemId, int quantity)
    {
        var item = await _db.CartItems
            .Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);

        if (item == null)
        {
            return OperationResult.Fail("Товар не найден в корзине");
        }

        if (quantity <= 0)
        {
            _db.CartItems.Remove(item);
        }
        else if (quantity > item.Product.Stock)
        {
            return OperationResult.Fail($"На складе осталось только {item.Product.Stock} шт.");
        }
        else
        {
            item.Quantity = quantity;
        }

        await _db.SaveChangesAsync();
        return OperationResult.Ok();
    }


    /// Оформляется заказ из корзины
    public async Task<OperationResult> PlaceOrderAsync(int userId, string phone, string? comment)
    {
        var cart = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cart.Count == 0)
        {
            return OperationResult.Fail("Корзина пуста");
        }

        foreach (var item in cart)
        {
            if (item.Quantity > item.Product.Stock)
            {
                return OperationResult.Fail($"Товара «{item.Product.Title}» осталось только {item.Product.Stock} шт.");
            }
        }

        var order = new Order
        {
            UserId = userId,
            Phone = phone.Trim(),
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
            Status = OrderStatus.New
        };

        foreach (var item in cart)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Product.Price
            });
            item.Product.Stock -= item.Quantity;
        }

        order.Total = order.Items.Sum(i => i.Price * i.Quantity);

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cart);
        await _db.SaveChangesAsync();

        return OperationResult.Ok();
    }
}
