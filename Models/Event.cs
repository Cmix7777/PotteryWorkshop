namespace PotteryWorkshop.Models;

public class EventItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime StartsAt { get; set; }
    public string Place { get; set; } = "";
    public int PlacesTotal { get; set; }
    public decimal Price { get; set; }
    public string Photo { get; set; } = "";
}