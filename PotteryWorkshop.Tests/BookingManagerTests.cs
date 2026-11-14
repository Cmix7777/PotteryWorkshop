using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Logic;
using PotteryWorkshop.Models;
using Xunit;

namespace PotteryWorkshop.Tests;

public class BookingManagerTests
{
    [Fact]
    public async Task Book_FreeLesson_CreatesNewBooking()
    {
        using var t = new TestDb();
        var user = t.AddUser("a@test.ru");
        var lesson = t.AddLesson(DateTime.Now.AddDays(2), places: 3);

        var result = await new BookingManager(t.Db).BookAsync(user.Id, lesson.Id);

        Assert.True(result.Success);
        var booking = await t.Db.Bookings.SingleAsync();
        Assert.Equal(BookingStatus.New, booking.Status);
    }

    [Fact]
    public async Task Book_NoFreePlaces_Fails()
    {
        using var t = new TestDb();
        var first = t.AddUser("a@test.ru");
        var second = t.AddUser("b@test.ru");
        var lesson = t.AddLesson(DateTime.Now.AddDays(2), places: 1);
        var manager = new BookingManager(t.Db);

        await manager.BookAsync(first.Id, lesson.Id);
        var result = await manager.BookAsync(second.Id, lesson.Id);

        Assert.False(result.Success);
        Assert.Equal(1, await t.Db.Bookings.CountAsync());
    }

    [Fact]
    public async Task Book_SameLessonTwice_Fails()
    {
        using var t = new TestDb();
        var user = t.AddUser("a@test.ru");
        var lesson = t.AddLesson(DateTime.Now.AddDays(2), places: 5);
        var manager = new BookingManager(t.Db);

        await manager.BookAsync(user.Id, lesson.Id);
        var result = await manager.BookAsync(user.Id, lesson.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Book_PastLesson_Fails()
    {
        using var t = new TestDb();
        var user = t.AddUser("a@test.ru");
        var lesson = t.AddLesson(DateTime.Now.AddDays(-1), places: 5);

        var result = await new BookingManager(t.Db).BookAsync(user.Id, lesson.Id);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task Cancel_FreesPlaceForOthers()
    {
        using var t = new TestDb();
        var first = t.AddUser("a@test.ru");
        var second = t.AddUser("b@test.ru");
        var lesson = t.AddLesson(DateTime.Now.AddDays(2), places: 1);
        var manager = new BookingManager(t.Db);

        await manager.BookAsync(first.Id, lesson.Id);
        var booking = await t.Db.Bookings.SingleAsync();
        await manager.CancelAsync(first.Id, booking.Id);
        var result = await manager.BookAsync(second.Id, lesson.Id);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task Cancel_SomeoneElsesBooking_Fails()
    {
        using var t = new TestDb();
        var owner = t.AddUser("a@test.ru");
        var stranger = t.AddUser("b@test.ru");
        var lesson = t.AddLesson(DateTime.Now.AddDays(2), places: 3);
        var manager = new BookingManager(t.Db);

        await manager.BookAsync(owner.Id, lesson.Id);
        var booking = await t.Db.Bookings.SingleAsync();
        var result = await manager.CancelAsync(stranger.Id, booking.Id);

        Assert.False(result.Success);
    }
}
