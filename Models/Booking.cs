namespace PotteryWorkshop.Models;

/// <summary>Запись пользователя на занятие (таблица bookings).</summary>
public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public BookingStatus Status { get; set; } = BookingStatus.New;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
