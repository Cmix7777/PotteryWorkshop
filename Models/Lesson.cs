namespace PotteryWorkshop.Models;

public class Lesson
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public Service? Service { get; set; }
    public DateTime StartsAt { get; set; }
    public int PlacesTotal { get; set; }
    public string Teacher { get; set; } = "";
}