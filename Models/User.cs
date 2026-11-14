using System.ComponentModel.DataAnnotations;

namespace PotteryWorkshop.Models;


public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Email { get; set; } = "";


    [Required]
    public string PasswordHash { get; set; } = "";

    [Required, MaxLength(100)]
    public string FullName { get; set; } = "";

    [MaxLength(20)]
    public string? Phone { get; set; }

    public UserRole Role { get; set; } = UserRole.User;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Booking> Bookings { get; set; } = new();
    public List<EventRegistration> EventRegistrations { get; set; } = new();
    public List<CartItem> CartItems { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}
