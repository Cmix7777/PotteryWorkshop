namespace PotteryWorkshop.Models;

public class EventRegistration
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int EventId { get; set; }
    public EventItem? Event { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}