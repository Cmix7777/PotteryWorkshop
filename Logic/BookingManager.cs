using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Logic;

/// Результат операции: успешно или текст ошибки
public class OperationResult
{
    public bool Success { get; private init; }
    public string? Error { get; private init; }

    public static OperationResult Ok() => new() { Success = true };
    public static OperationResult Fail(string error) => new() { Success = false, Error = error };
}

/// Модуль записи на занятия
public class BookingManager
{
    private readonly AppDbContext _db;

    public BookingManager(AppDbContext db)
    {
        _db = db;
    }

    /// Сколько мест осталось: всего мест минус неотменённые записи
    public static int FreePlaces(Lesson lesson)
    {
        return lesson.PlacesTotal - lesson.Bookings.Count(b => b.Status != BookingStatus.Cancelled);
    }

    /// Записывает пользователя на занятие, если это разрешено правилами
    public async Task<OperationResult> BookAsync(int userId, int lessonId)
    {
        var lesson = await _db.Lessons
            .Include(l => l.Bookings)
            .FirstOrDefaultAsync(l => l.Id == lessonId);

        if (lesson == null)
        {
            return OperationResult.Fail("Занятие не найдено");
        }

        if (lesson.StartsAt <= DateTime.Now)
        {
            return OperationResult.Fail("Нельзя записаться на занятие, которое уже прошло");
        }

        var alreadyBooked = lesson.Bookings.Any(b => b.UserId == userId && b.Status != BookingStatus.Cancelled);
        if (alreadyBooked)
        {
            return OperationResult.Fail("Вы уже записаны на это занятие");
        }

        if (FreePlaces(lesson) <= 0)
        {
            return OperationResult.Fail("На это занятие не осталось свободных мест");
        }

        _db.Bookings.Add(new Booking
        {
            UserId = userId,
            LessonId = lessonId,
            Status = BookingStatus.New
        });
        await _db.SaveChangesAsync();

        return OperationResult.Ok();
    }

    /// Отменяет запись. Клиент может отменить только свою запись и только до начала занятия
    public async Task<OperationResult> CancelAsync(int userId, int bookingId)
    {
        var booking = await _db.Bookings
            .Include(b => b.Lesson)
            .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);

        if (booking == null)
        {
            return OperationResult.Fail("Запись не найдена");
        }

        if (booking.Status != BookingStatus.New && booking.Status != BookingStatus.Confirmed)
        {
            return OperationResult.Fail("Эту запись уже нельзя отменить");
        }

        if (booking.Lesson.StartsAt <= DateTime.Now)
        {
            return OperationResult.Fail("Занятие уже началось или прошло");
        }

        booking.Status = BookingStatus.Cancelled;
        await _db.SaveChangesAsync();

        return OperationResult.Ok();
    }
}
