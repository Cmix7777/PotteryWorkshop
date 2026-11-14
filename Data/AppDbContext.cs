using Microsoft.EntityFrameworkCore;
using PotteryWorkshop.Models;

namespace PotteryWorkshop.Data;

/// Контекст базы данных сайта. Каждый DbSet соответствует таблице из схемы БД
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<FeedbackMessage> FeedbackMessages => Set<FeedbackMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Имена таблиц как в схеме БД
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<Service>().ToTable("services");
        modelBuilder.Entity<Lesson>().ToTable("lessons");
        modelBuilder.Entity<Booking>().ToTable("bookings");
        modelBuilder.Entity<Event>().ToTable("events");
        modelBuilder.Entity<EventRegistration>().ToTable("event_registrations");
        modelBuilder.Entity<Category>().ToTable("categories");
        modelBuilder.Entity<Product>().ToTable("products");
        modelBuilder.Entity<CartItem>().ToTable("cart_items");
        modelBuilder.Entity<Order>().ToTable("orders");
        modelBuilder.Entity<OrderItem>().ToTable("order_items");
        modelBuilder.Entity<Review>().ToTable("reviews");
        modelBuilder.Entity<FeedbackMessage>().ToTable("feedback_messages");

        // Email уникальный: два аккаунта на один адрес создать нельзя
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Перечисления храним строками, чтобы в базе было видно "Admin", а не 2
        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Service>().Property(s => s.Kind).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Booking>().Property(b => b.Status).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Order>().Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Review>().Property(r => r.Status).HasConversion<string>().HasMaxLength(20);

        // Один пользователь не может дважды зарегистрироваться на одно мероприятие
        modelBuilder.Entity<EventRegistration>().HasIndex(r => new { r.UserId, r.EventId }).IsUnique();

        // Один товар лежит в корзине одной строкой, меняется только количество
        modelBuilder.Entity<CartItem>().HasIndex(c => new { c.UserId, c.ProductId }).IsUnique();

        // Товар, который уже есть в заказах, удалить нельзя, иначе пропадёт история заказов
        modelBuilder.Entity<OrderItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
