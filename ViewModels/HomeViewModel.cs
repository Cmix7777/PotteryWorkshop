using PotteryWorkshop.Models;

namespace PotteryWorkshop.ViewModels;


public class HomeViewModel
{
    public List<Service> Services { get; set; } = new();
    public List<EventCardViewModel> Events { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}


public class EventCardViewModel
{
    public Event Event { get; set; } = null!;
    public int FreePlaces { get; set; }
}
