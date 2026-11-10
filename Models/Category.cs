using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;

/// <summary>Категория товаров: изделия, материалы, инструменты (таблица categories).</summary>
public class Category
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = "";

    public List<Product> Products { get; set; } = new();
}
