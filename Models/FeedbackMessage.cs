using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;


public class FeedbackMessage
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [Required, MaxLength(100)]
    public string Email { get; set; } = "";

    [MaxLength(50)]
    public string Topic { get; set; } = "";

    [Required, MaxLength(2000)]
    public string Message { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
