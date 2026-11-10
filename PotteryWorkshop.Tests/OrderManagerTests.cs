using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Logic;
using PotteryWorkshop.Models;
using Xunit;

namespace PotteryWorkshop.Tests;

/// <summary>Проверки модуля корзины и заказов.</summary>
public class OrderManagerTests
{
    [Fact]
    public async Task AddToCart_MoreThanStock_Fails()
    {
        using var t = new TestDb();
        var user = t.AddUser("a@test.ru");
        var product = t.AddProduct(price: 1000, stock: 2);

        var result = await new OrderManager(t.Db).AddToCartAsync(user.Id, product.Id, 3);

        Assert.False(result.Success);
        Assert.Equal(0, await t.Db.CartItems.CountAsync());
    }

    [Fact]
    public async Task AddToCart_ZeroQuantity_Fails()
    {
        using var t = new TestDb();
        var user = t.AddUser("a@test.ru");
        var product = t.AddProduct(price: 1000, stock: 2);

        var result = await new OrderManager(t.Db).AddToCartAsync(user.Id, product.Id, 0);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task PlaceOrder_EmptyCart_Fails()
    {
        using var t = new TestDb();
        var user = t.AddUser("a@test.ru");

        var result = await new OrderManager(t.Db).PlaceOrderAsync(user.Id, "+79120000000", null);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task PlaceOrder_SavesTotalReducesStockAndClearsCart()
    {
        using var t = new TestDb();
        var user = t.AddUser("a@test.ru");
        var product = t.AddProduct(price: 1200, stock: 5);
        var manager = new OrderManager(t.Db);

        await manager.AddToCartAsync(user.Id, product.Id, 2);
        var result = await manager.PlaceOrderAsync(user.Id, "+79120000000", "заберу вечером");

        Assert.True(result.Success);
        var order = await t.Db.Orders.Include(o => o.Items).SingleAsync();
        Assert.Equal(2400, order.Total);
        Assert.Equal(OrderStatus.New, order.Status);
        Assert.Equal(1200, order.Items.Single().Price);
        Assert.Equal(3, (await t.Db.Products.SingleAsync()).Stock);
        Assert.Equal(0, await t.Db.CartItems.CountAsync());
    }
}
