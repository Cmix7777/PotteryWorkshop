using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;

/// <summary>Заказ в магазине (таблица orders).</summary>
public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.New;

    /// <summary>Сумма заказа в рублях.</summary>
    public int Total { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; } = "";

    [MaxLength(500)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<OrderItem> Items { get; set; } = new();
}
