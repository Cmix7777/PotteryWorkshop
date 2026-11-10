using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Logic;

/// <summary>Модуль отзывов. По ТЗ отзыв можно оставить только после посещения занятия или покупки.</summary>
public class ReviewManager
{
    private readonly AppDbContext _db;

    public ReviewManager(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>Есть ли у пользователя посещённое занятие или выданный заказ.</summary>
    public async Task<bool> CanLeaveReviewAsync(int userId)
    {
        var visited = await _db.Bookings.AnyAsync(b => b.UserId == userId && b.Status == BookingStatus.Visited);
        var bought = await _db.Orders.AnyAsync(o => o.UserId == userId && o.Status == OrderStatus.Issued);
        return visited || bought;
    }

    /// <summary>Сохраняет отзыв со статусом «на модерации».</summary>
    public async Task<OperationResult> AddAsync(int userId, string serviceType, int rating, string text)
    {
        if (!await CanLeaveReviewAsync(userId))
        {
            return OperationResult.Fail("Отзыв можно оставить после посещения занятия или получения заказа");
        }

        if (rating < 1 || rating > 5)
        {
            return OperationResult.Fail("Оценка должна быть от 1 до 5");
        }

        _db.Reviews.Add(new Review
        {
            UserId = userId,
            ServiceType = serviceType,
            Rating = rating,
            Text = text.Trim(),
            Status = ReviewStatus.Moderation
        });
        await _db.SaveChangesAsync();

        return OperationResult.Ok();
    }
}
