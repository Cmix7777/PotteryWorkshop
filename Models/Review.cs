using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;

/// <summary>Отзыв клиента (таблица reviews). На сайте виден только после модерации.</summary>
public class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required, MaxLength(1000)]
    public string Text { get; set; } = "";

    /// <summary>Оценка от 1 до 5.</summary>
    [Range(1, 5)]
    public int Rating { get; set; }

    /// <summary>Где был клиент: занятие, мастер-класс, урок или покупка. По этому полю работает фильтр.</summary>
    [MaxLength(100)]
    public string ServiceType { get; set; } = "";

    public ReviewStatus Status { get; set; } = ReviewStatus.Moderation;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
