using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;

public class Product
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    [Required, MaxLength(150)]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";


    public int Price { get; set; }


    public int Stock { get; set; }

    public string? PhotoUrl { get; set; }

    public bool IsFeatured { get; set; }
}
