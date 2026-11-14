using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;


public class Lesson
{
    public int Id { get; set; }

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public DateTime StartsAt { get; set; }

    /// <summary>Сколько всего мест на занятии.</summary>
    public int PlacesTotal { get; set; }

    [MaxLength(100)]
    public string Teacher { get; set; } = "";

    public List<Booking> Bookings { get; set; } = new();
}
