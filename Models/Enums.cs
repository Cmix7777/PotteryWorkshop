namespace PotteryWorkshop.Models;

/// <summary>Роль пользователя на сайте.</summary>
public enum UserRole
{
    /// <summary>Обычный зарегистрированный пользователь.</summary>
    User,
    /// <summary>Модератор: расписание, мероприятия, записи.</summary>
    Moderator,
    /// <summary>Администратор: полный доступ.</summary>
    Admin
}

/// <summary>Вид услуги мастерской.</summary>
public enum ServiceKind
{
    /// <summary>Групповое занятие на гончарном круге.</summary>
    Lesson,
    /// <summary>Разовый мастер-класс.</summary>
    MasterClass,
    /// <summary>Индивидуальный урок с мастером.</summary>
    Individual
}

/// <summary>Статус записи на занятие.</summary>
public enum BookingStatus
{
    New,
    Confirmed,
    Visited,
    Cancelled
}

/// <summary>Статус заказа в магазине.</summary>
public enum OrderStatus
{
    New,
    Confirmed,
    ReadyForPickup,
    Issued,
    Cancelled
}

/// <summary>Статус отзыва.</summary>
public enum ReviewStatus
{
    Moderation,
    Published,
    Hidden
}
