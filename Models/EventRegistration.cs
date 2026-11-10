namespace PotteryWorkshop.Models;

/// <summary>Регистрация пользователя на мероприятие (таблица event_registrations).</summary>
public class EventRegistration
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int EventId { get; set; }
    public Event Event { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
