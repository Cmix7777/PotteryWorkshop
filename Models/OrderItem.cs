namespace PotteryWorkshop.Models;

/// <summary>Строка заказа (таблица order_items). Цена сохраняется на момент покупки.</summary>
public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    /// <summary>Цена за штуку на момент оформления заказа.</summary>
    public int Price { get; set; }
}
