using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;

/// <summary>Услуга мастерской: занятие, мастер-класс или индивидуальный урок (таблица services).</summary>
public class Service
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    public ServiceKind Kind { get; set; }

    /// <summary>Цена в рублях.</summary>
    public int Price { get; set; }

    public int DurationMinutes { get; set; }

    public string? PhotoUrl { get; set; }

    /// <summary>Занятия этой услуги в расписании.</summary>
    public List<Lesson> Lessons { get; set; } = new();
}
