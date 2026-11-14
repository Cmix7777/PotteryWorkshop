using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;

public class Event
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    public DateTime StartsAt { get; set; }

    [MaxLength(150)]
    public string Place { get; set; } = "";

    public int PlacesTotal { get; set; }


    public int Price { get; set; }

    public string? PhotoUrl { get; set; }

    public List<EventRegistration> Registrations { get; set; } = new();
}
