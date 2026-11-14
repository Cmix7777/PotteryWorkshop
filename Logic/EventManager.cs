using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Logic;

/// Модуль регистрации на мероприятия
public class EventManager
{
    private readonly AppDbContext _db;

    public EventManager(AppDbContext db)
    {
        _db = db;
    }

    public static int FreePlaces(Event ev)
    {
        return ev.PlacesTotal - ev.Registrations.Count;
    }

    /// Регистрирует пользователя, если мероприятие не прошло, есть места и он ещё не зарегистрирован
    public async Task<OperationResult> RegisterAsync(int userId, int eventId)
    {
        var ev = await _db.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (ev == null)
        {
            return OperationResult.Fail("Мероприятие не найдено");
        }

        if (ev.StartsAt <= DateTime.Now)
        {
            return OperationResult.Fail("Мероприятие уже прошло");
        }

        if (ev.Registrations.Any(r => r.UserId == userId))
        {
            return OperationResult.Fail("Вы уже зарегистрированы на это мероприятие");
        }

        if (FreePlaces(ev) <= 0)
        {
            return OperationResult.Fail("Свободных мест нет");
        }

        _db.EventRegistrations.Add(new EventRegistration { UserId = userId, EventId = eventId });
        await _db.SaveChangesAsync();

        return OperationResult.Ok();
    }

    /// Отменяет регистрацию на будущее мероприятие
    public async Task<OperationResult> UnregisterAsync(int userId, int eventId)
    {
        var registration = await _db.EventRegistrations
            .Include(r => r.Event)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.EventId == eventId);

        if (registration == null)
        {
            return OperationResult.Fail("Регистрация не найдена");
        }

        if (registration.Event.StartsAt <= DateTime.Now)
        {
            return OperationResult.Fail("Мероприятие уже прошло");
        }

        _db.EventRegistrations.Remove(registration);
        await _db.SaveChangesAsync();

        return OperationResult.Ok();
    }
}
