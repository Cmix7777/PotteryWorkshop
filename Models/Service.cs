using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;


public class Service
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    public ServiceKind Kind { get; set; }


    public int Price { get; set; }

    public int DurationMinutes { get; set; }

    public string? PhotoUrl { get; set; }


    public List<Lesson> Lessons { get; set; } = new();
}
