namespace PotteryWorkshop.Models;

public class Service
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Kind { get; set; } = ""; // занятие / мастер-класс / урок
    public decimal Price { get; set; }
    public int DurationMin { get; set; }
    public string Photo { get; set; } = "";
}