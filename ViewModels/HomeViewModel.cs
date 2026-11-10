using PotteryWorkshop.Models;

namespace PotteryWorkshop.ViewModels;

/// <summary>Всё, что показывается на главной странице.</summary>
public class HomeViewModel
{
    public List<Service> Services { get; set; } = new();
    public List<EventCardViewModel> Events { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}

/// <summary>Карточка мероприятия на главной: само мероприятие и сколько осталось мест.</summary>
public class EventCardViewModel
{
    public Event Event { get; set; } = null!;
    public int FreePlaces { get; set; }
}
