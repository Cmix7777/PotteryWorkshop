using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Data;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Tests;

/// <summary>
/// Тестовая база SQLite в памяти. Для каждого теста создаётся новая чистая база,
/// поэтому тесты не влияют друг на друга.
/// </summary>
public sealed class TestDb : IDisposable
{
    private readonly SqliteConnection _connection;
    public AppDbContext Db { get; }

    public TestDb()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Db = new AppDbContext(options);
        Db.Database.EnsureCreated();
    }

    public User AddUser(string email)
    {
        var user = new User { Email = email, FullName = email, PasswordHash = "hash" };
        Db.Users.Add(user);
        Db.SaveChanges();
        return user;
    }

    public Lesson AddLesson(DateTime startsAt, int places)
    {
        var service = new Service { Title = "Мастер-класс", Kind = ServiceKind.MasterClass, Price = 2000, DurationMinutes = 180 };
        var lesson = new Lesson { Service = service, StartsAt = startsAt, PlacesTotal = places, Teacher = "Тест" };
        Db.Lessons.Add(lesson);
        Db.SaveChanges();
        return lesson;
    }

    public Product AddProduct(int price, int stock)
    {
        var category = new Category { Name = "Готовые изделия" };
        var product = new Product { Category = category, Title = "Кружка", Price = price, Stock = stock };
        Db.Products.Add(product);
        Db.SaveChanges();
        return product;
    }

    public void Dispose()
    {
        Db.Dispose();
        _connection.Dispose();
    }
}
