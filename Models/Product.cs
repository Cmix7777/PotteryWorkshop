using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;

/// <summary>Товар магазина (таблица products).</summary>
public class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    [Required, MaxLength(150)]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    /// <summary>Цена в рублях.</summary>
    public int Price { get; set; }

    /// <summary>Остаток на складе.</summary>
    public int Stock { get; set; }

    public string? PhotoUrl { get; set; }

    /// <summary>Показывать ли товар на главной странице.</summary>
    public bool IsFeatured { get; set; }
}
