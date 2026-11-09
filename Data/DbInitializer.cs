using System;
using System.Linq;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Если база уже не пуста, ничего не делаем
        if (context.Services.Any())
        {
            return; 
        }

        // Добавляем тестовые услуги
        var services = new Service[]
        {
            new Service { Title = "Занятия на гончарном круге", Description = "Курс из 8 занятий: центровка глины, вытягивание стенок, первые чашки и тарелки.", Kind = "Курс", Price = 1500, DurationMin = 120, Photo = "wheel.jpg" },
            new Service { Title = "Мастер-классы", Description = "Разовые встречи на один вечер для тех, кто хочет попробовать. Можно прийти вдвоём.", Kind = "Мастер-класс", Price = 2500, DurationMin = 180, Photo = "mk.jpg" },
            new Service { Title = "Индивидуальные уроки", Description = "Занятие один на один с мастером, программа под ваш уровень и задачу.", Kind = "Индивидуально", Price = 3500, DurationMin = 90, Photo = "indiv.jpg" }
        };

        foreach (var s in services)
        {
            context.Services.Add(s);
        }

        // Добавляем тестовые товары
        var categories = new Category[]
        {
            new Category { Name = "Готовые изделия" },
            new Category { Name = "Материалы" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges(); // Сохраняем, чтобы получить ID категорий

        var products = new Product[]
        {
            new Product { CategoryId = 1, Title = "Кружка «Охра», 350 мл", Description = "Ручная работа, пищевая глазурь", Price = 1200, Stock = 5, Photo = "mug.jpg", IsFeatured = true },
            new Product { CategoryId = 1, Title = "Набор пиал, 2 шт.", Description = "Идеально для чая или десертов", Price = 1900, Stock = 3, Photo = "bowls.jpg", IsFeatured = true },
            new Product { CategoryId = 2, Title = "Глина белая, 5 кг", Description = "Профессиональная гончарная глина", Price = 650, Stock = 20, Photo = "clay.jpg", IsFeatured = false }
        };

        foreach (var p in products)
        {
            context.Products.Add(p);
        }
        // Добавляем тестовые записи (bookings) для демонстрации
        var bookings = new Booking[]
        {
            new Booking { UserId = 1, ServiceId = 1, FullName = "Тестовый пользователь", Phone = "+7 (900) 123-45-67", Email = "test@test.ru", PreferredDate = DateTime.Now.AddDays(7), Comment = "Хочу попробовать", Status = "подтверждена" }
        };

        foreach (var b in bookings)
        {
            context.Bookings.Add(b);
        }
        // Тестовые отзывы
        var reviews = new Review[]
        {
            new Review { UserId = 1, Text = "Пришла без опыта и боялась, что ничего не выйдет. Мастер всё показал руками, через неделю забрала свою первую кружку. Атмосфера очень тёплая!", Rating = 5, ServiceType = "Мастер-класс «Первый горшок»", Status = "опубликован", CreatedAt = DateTime.Now.AddDays(-15) },
            new Review { UserId = 1, Text = "Хожу на курс второй месяц. Нравится, что группы маленькие и у каждого свой круг, никто не ждёт очереди.", Rating = 5, ServiceType = "Занятия на гончарном круге", Status = "опубликован", CreatedAt = DateTime.Now.AddDays(-25) },
            new Review { UserId = 1, Text = "Купила набор пиал в подарок маме, упаковали аккуратно. Глазурь вживую ещё красивее, чем на фото.", Rating = 5, ServiceType = "Покупка в магазине", Status = "опубликован", CreatedAt = DateTime.Now.AddDays(-30) },
            new Review { UserId = 1, Text = "Хороший формат для тех, кто хочет быстро прокачать навык. Единственное — хотелось бы чуть больше времени на занятие.", Rating = 4, ServiceType = "Индивидуальный урок", Status = "опубликован", CreatedAt = DateTime.Now.AddDays(-40) }
        };

        foreach (var r in reviews)
        {
            context.Reviews.Add(r);
        }
        context.SaveChanges();
    }
}