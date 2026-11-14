using System.ComponentModel.DataAnnotations;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.ViewModels;

public class FeedbackViewModel
{
    [Required(ErrorMessage = "Введите имя")]
    [StringLength(100)]
    [Display(Name = "Ваше имя")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Неверный формат email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [Display(Name = "Тема обращения")]
    public string Topic { get; set; } = "Общий вопрос";

    [Required(ErrorMessage = "Напишите сообщение")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Сообщение должно быть от 10 до 2000 символов")]
    [Display(Name = "Сообщение")]
    public string Message { get; set; } = "";
}


public class CheckoutViewModel
{
    [Required(ErrorMessage = "Введите телефон для связи")]
    [Phone(ErrorMessage = "Неверный формат телефона")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = "";

    [StringLength(500)]
    [Display(Name = "Комментарий к заказу")]
    public string? Comment { get; set; }
}


public class ReviewFormViewModel
{
    [Required(ErrorMessage = "Выберите, где вы были")]
    [Display(Name = "Где вы были")]
    public string ServiceType { get; set; } = "";

    [Range(1, 5, ErrorMessage = "Оценка от 1 до 5")]
    [Display(Name = "Оценка")]
    public int Rating { get; set; } = 5;

    [Required(ErrorMessage = "Напишите отзыв")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Отзыв должен быть от 10 до 1000 символов")]
    [Display(Name = "Ваш отзыв")]
    public string Text { get; set; } = "";


    public static readonly string[] ServiceTypes =
    {
        "Занятия на гончарном круге",
        "Мастер-класс",
        "Индивидуальный урок",
        "Покупка в магазине"
    };
}


public class ProfileViewModel
{
    [Required(ErrorMessage = "Введите имя")]
    [StringLength(100)]
    [Display(Name = "Имя и фамилия")]
    public string FullName { get; set; } = "";

    [Phone(ErrorMessage = "Неверный формат телефона")]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    public string Email { get; set; } = "";
}


public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Выберите категорию")]
    [Display(Name = "Категория")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Введите название")]
    [StringLength(150)]
    [Display(Name = "Название")]
    public string Title { get; set; } = "";

    [StringLength(1000)]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Range(1, 1000000, ErrorMessage = "Цена от 1 до 1 000 000 ₽")]
    [Display(Name = "Цена, ₽")]
    public int Price { get; set; }

    [Range(0, 10000, ErrorMessage = "Остаток от 0 до 10 000")]
    [Display(Name = "Остаток на складе")]
    public int Stock { get; set; }

    [Display(Name = "Показывать на главной")]
    public bool IsFeatured { get; set; }
}


public class LessonFormViewModel
{
    [Required(ErrorMessage = "Выберите услугу")]
    [Display(Name = "Услуга")]
    public int ServiceId { get; set; }

    [Required(ErrorMessage = "Укажите дату и время")]
    [Display(Name = "Дата и время")]
    public DateTime StartsAt { get; set; } = DateTime.Today.AddDays(1).AddHours(18);

    [Range(1, 50, ErrorMessage = "Мест от 1 до 50")]
    [Display(Name = "Мест")]
    public int PlacesTotal { get; set; } = 6;

    [Required(ErrorMessage = "Укажите мастера")]
    [StringLength(100)]
    [Display(Name = "Мастер")]
    public string Teacher { get; set; } = "";
}


public class EventFormViewModel
{
    [Required(ErrorMessage = "Введите название")]
    [StringLength(150)]
    [Display(Name = "Название")]
    public string Title { get; set; } = "";

    [StringLength(1000)]
    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Укажите дату и время")]
    [Display(Name = "Дата и время")]
    public DateTime StartsAt { get; set; } = DateTime.Today.AddDays(7).AddHours(18);

    [StringLength(150)]
    [Display(Name = "Место")]
    public string? Place { get; set; }

    [Range(1, 200, ErrorMessage = "Мест от 1 до 200")]
    [Display(Name = "Мест")]
    public int PlacesTotal { get; set; } = 10;

    [Range(0, 100000, ErrorMessage = "Цена от 0 до 100 000 ₽")]
    [Display(Name = "Цена, ₽")]
    public int Price { get; set; }
}

public class AdminStatsViewModel
{
    public int UsersCount { get; set; }
    public int NewBookings { get; set; }
    public int UpcomingLessons { get; set; }
    public int NewOrders { get; set; }
    public int Revenue { get; set; }
    public int ReviewsOnModeration { get; set; }
    public int NewMessages { get; set; }
    public List<(string Service, int Count)> PopularServices { get; set; } = new();
    public List<Product> LowStock { get; set; } = new();
}
