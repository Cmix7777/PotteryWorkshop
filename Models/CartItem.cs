namespace PotteryWorkshop.Models;

/// <summary>Товар в корзине пользователя (таблица cart_items).</summary>
public class CartItem
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int Quantity { get; set; } = 1;
}
