namespace PotteryWorkshop.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int ServiceId { get; set; }  // Заменили LessonId на ServiceId для простоты
    public Service? Service { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime PreferredDate { get; set; }
    public string Comment { get; set; } = "";
    public string Status { get; set; } = "новая";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}