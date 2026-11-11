using Microsoft.AspNetCore.Identity;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Data;

/// <summary>Заполняет пустую базу тестовыми данными, чтобы сайт было что показать сразу после запуска.</summary>
public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Если пользователи уже есть, база заполнена, второй раз не добавляем
        if (db.Users.Any())
        {
            return;
        }

        var hasher = new PasswordHasher<User>();

        var admin = new User { Email = "admin@krug.ru", FullName = "Администратор", Role = UserRole.Admin };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

        var moderator = new User { Email = "moder@krug.ru", FullName = "Анна Петрова", Role = UserRole.Moderator };
        moderator.PasswordHash = hasher.HashPassword(moderator, "Moder123!");

        var maria = new User { Email = "maria@example.com", FullName = "Мария Смирнова", Phone = "+79120000000" };
        maria.PasswordHash = hasher.HashPassword(maria, "User123!");

        var dmitry = new User { Email = "dmitry@example.com", FullName = "Дмитрий Козлов" };
        dmitry.PasswordHash = hasher.HashPassword(dmitry, "User123!");

        var elena = new User { Email = "elena@example.com", FullName = "Елена Волкова" };
        elena.PasswordHash = hasher.HashPassword(elena, "User123!");

        db.Users.AddRange(admin, moderator, maria, dmitry, elena);

        var wheel = new Service
        {
            Title = "Занятия на гончарном круге",
            Description = "Курс из 8 занятий: центровка глины, вытягивание стенок, первые чашки и тарелки.",
            Kind = ServiceKind.Lesson, Price = 1500, DurationMinutes = 120, PhotoUrl = "/img/service-wheel.jpg"
        };
        var master = new Service
        {
            Title = "Мастер-классы",
            Description = "Разовые встречи на один вечер для тех, кто хочет попробовать. Можно прийти вдвоём.",
            Kind = ServiceKind.MasterClass, Price = 2000, DurationMinutes = 180, PhotoUrl = "/img/service-masterclass.jpg"
        };
        var personal = new Service
        {
            Title = "Индивидуальные уроки",
            Description = "Занятие один на один с мастером, программа под ваш уровень и задачу.",
            Kind = ServiceKind.Individual, Price = 3500, DurationMinutes = 90, PhotoUrl = "/img/service-individual.jpg"
        };
        db.Services.AddRange(wheel, master, personal);

        // Расписание строим от сегодняшней даты, чтобы занятия всегда были в будущем
        var today = DateTime.Today;
        db.Lessons.AddRange(
            new Lesson { Service = wheel, StartsAt = today.AddDays(2).AddHours(11), PlacesTotal = 6, Teacher = "Иван Сидоров" },
            new Lesson { Service = wheel, StartsAt = today.AddDays(4).AddHours(14), PlacesTotal = 6, Teacher = "Иван Сидоров" },
            new Lesson { Service = master, StartsAt = today.AddDays(5).AddHours(18), PlacesTotal = 8, Teacher = "Анна Петрова" },
            new Lesson { Service = personal, StartsAt = today.AddDays(3).AddHours(16), PlacesTotal = 1, Teacher = "Иван Сидоров" });

        db.Events.AddRange(
            new Event
            {
                Title = "Первый горшок за вечер",
                Description = "Знакомство с кругом для новичков, каждый уходит со своей чашкой.",
                StartsAt = today.AddDays(7).AddHours(18), Place = "Большой зал", PlacesTotal = 8, Price = 2000, PhotoUrl = "/img/event-first-pot.jpg"
            },
            new Event
            {
                Title = "Роспись керамики",
                Description = "Расписываем готовые тарелки ангобами и глазурью, потом обжиг.",
                StartsAt = today.AddDays(14).AddHours(12), Place = "Малый зал", PlacesTotal = 10, Price = 1800, PhotoUrl = "/img/event-painting.jpg"
            },
            new Event
            {
                Title = "Новогодние игрушки из глины",
                Description = "Лепим и раскрашиваем ёлочные игрушки, подходит детям от 7 лет.",
                StartsAt = today.AddDays(21).AddHours(17), Place = "Большой зал", PlacesTotal = 12, Price = 1500, PhotoUrl = "/img/event-newyear.jpg"
            });

        var ready = new Category { Name = "Готовые изделия" };
        var materials = new Category { Name = "Материалы" };
        var tools = new Category { Name = "Инструменты" };
        db.Categories.AddRange(ready, materials, tools);

        var mug = new Product { Category = ready, Title = "Кружка «Охра», 350 мл", Price = 1200, Stock = 5, IsFeatured = true, PhotoUrl = "/img/product-mug.jpg",
            Description = "Кружка ручной работы, покрыта матовой глазурью охристого цвета. Можно мыть в посудомоечной машине." };
        var bowls = new Product { Category = ready, Title = "Набор пиал, 2 шт.", Price = 1900, Stock = 3, IsFeatured = true, PhotoUrl = "/img/product-bowls.jpg",
            Description = "Две пиалы по 250 мл для чая или супа, глазурь «морская волна» с переходом цвета." };
        db.Products.AddRange(
            mug,
            bowls,
            new Product { Category = ready, Title = "Ваза «Терракота», 25 см", Price = 2400, Stock = 2, PhotoUrl = "/img/product-vase.jpg",
                Description = "Ваза из красной глины без глазури снаружи, внутри покрыта прозрачной глазурью и держит воду." },
            new Product { Category = ready, Title = "Тарелка «Лес», 22 см", Price = 1600, Stock = 4, PhotoUrl = "/img/product-plate.jpg",
                Description = "Обеденная тарелка с оттиском листьев папоротника по краю." },
            new Product { Category = materials, Title = "Глина белая, 5 кг", Price = 650, Stock = 20, IsFeatured = true, PhotoUrl = "/img/product-clay.jpg",
                Description = "Пластичная белая глина для гончарного круга и ручной лепки, температура обжига 1200 °C." },
            new Product { Category = materials, Title = "Глазурь прозрачная, 1 л", Price = 480, Stock = 10, PhotoUrl = "/img/product-glaze.jpg",
                Description = "Прозрачная блестящая глазурь для посуды, безопасна для контакта с пищей." },
            new Product { Category = tools, Title = "Набор стеков, 8 шт.", Price = 890, Stock = 7, IsFeatured = true, PhotoUrl = "/img/product-tools.jpg",
                Description = "Деревянные и металлические стеки для лепки, резьбы и выравнивания стенок." },
            new Product { Category = tools, Title = "Нож для резки глины", Price = 240, Stock = 15, PhotoUrl = "/img/product-knife.jpg",
                Description = "Струна с деревянными ручками для отрезания изделия от круга." });

        // Прошедшее занятие и выданный заказ у Марии: по ним она может оставлять отзывы
        var pastLesson = new Lesson { Service = master, StartsAt = today.AddDays(-7).AddHours(18), PlacesTotal = 8, Teacher = "Анна Петрова" };
        db.Lessons.Add(pastLesson);
        db.Bookings.Add(new Booking { User = maria, Lesson = pastLesson, Status = BookingStatus.Visited, CreatedAt = today.AddDays(-10) });

        var order = new Order { User = maria, Status = OrderStatus.Issued, Phone = "+79120000000", Total = 1900, CreatedAt = today.AddDays(-12) };
        order.Items.Add(new OrderItem { Product = bowls, Quantity = 1, Price = 1900 });
        db.Orders.Add(order);

        db.Reviews.AddRange(
            new Review
            {
                User = maria, Rating = 5, Status = ReviewStatus.Published, ServiceType = "Мастер-класс",
                Text = "Пришла без опыта и боялась, что ничего не выйдет. Мастер всё показал руками, через неделю забрала свою первую кружку."
            },
            new Review
            {
                User = dmitry, Rating = 5, Status = ReviewStatus.Published, ServiceType = "Занятия на гончарном круге",
                Text = "Хожу на курс второй месяц. Нравится, что группы маленькие и у каждого свой круг."
            },
            new Review
            {
                User = elena, Rating = 4, Status = ReviewStatus.Published, ServiceType = "Покупка в магазине",
                Text = "Купила набор пиал в подарок маме, упаковали аккуратно. Глазурь вживую ещё красивее, чем на фото."
            });

        db.SaveChanges();
    }
}
