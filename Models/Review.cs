namespace PotteryWorkshop.Models;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Text { get; set; } = "";
    public int Rating { get; set; } // 1-5
    public string ServiceType { get; set; } = ""; // где был: "Мастер-класс", "Магазин" и т.д.
    public string Status { get; set; } = "на модерации"; // на модерации / опубликован / скрыт
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}