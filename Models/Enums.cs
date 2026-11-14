namespace PotteryWorkshop.Models;

/// Роль пользователя на сайте
public enum UserRole
{
    User,
    Moderator,
    Admin
}

/// Вид услуги мастерской
public enum ServiceKind
{
    Lesson,
    MasterClass,
    Individual
}

/// Статус записи на занятие
public enum BookingStatus
{
    New,
    Confirmed,
    Visited,
    Cancelled
}

/// Статус заказа в магазине
public enum OrderStatus
{
    New,
    Confirmed,
    ReadyForPickup,
    Issued,
    Cancelled
}

/// Статус отзыва
public enum ReviewStatus
{
    Moderation,
    Published,
    Hidden
}
