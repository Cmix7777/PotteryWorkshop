namespace PotteryWorkshop.Models;

/// <summary>Русские названия статусов и видов услуг для вывода на страницах.</summary>
public static class StatusNames
{
    public static string ToRussian(this BookingStatus status) => status switch
    {
        BookingStatus.New => "Новая",
        BookingStatus.Confirmed => "Подтверждена",
        BookingStatus.Visited => "Посещена",
        BookingStatus.Cancelled => "Отменена",
        _ => status.ToString()
    };

    public static string ToRussian(this OrderStatus status) => status switch
    {
        OrderStatus.New => "Новый",
        OrderStatus.Confirmed => "Подтверждён",
        OrderStatus.ReadyForPickup => "Готов к выдаче",
        OrderStatus.Issued => "Выдан",
        OrderStatus.Cancelled => "Отменён",
        _ => status.ToString()
    };

    public static string ToRussian(this ReviewStatus status) => status switch
    {
        ReviewStatus.Moderation => "На модерации",
        ReviewStatus.Published => "Опубликован",
        ReviewStatus.Hidden => "Скрыт",
        _ => status.ToString()
    };

    public static string ToRussian(this UserRole role) => role switch
    {
        UserRole.User => "Пользователь",
        UserRole.Moderator => "Модератор",
        UserRole.Admin => "Администратор",
        _ => role.ToString()
    };

    public static string ToRussian(this ServiceKind kind) => kind switch
    {
        ServiceKind.Lesson => "Занятие на круге",
        ServiceKind.MasterClass => "Мастер-класс",
        ServiceKind.Individual => "Индивидуальный урок",
        _ => kind.ToString()
    };
}
