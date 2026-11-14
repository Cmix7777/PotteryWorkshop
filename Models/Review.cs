using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;


public class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required, MaxLength(1000)]
    public string Text { get; set; } = "";


    [Range(1, 5)]
    public int Rating { get; set; }


    [MaxLength(100)]
    public string ServiceType { get; set; } = "";

    public ReviewStatus Status { get; set; } = ReviewStatus.Moderation;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
