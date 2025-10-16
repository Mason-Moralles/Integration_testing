using FinalTas.Domain.Entities;
using BCrypt.Net;

namespace FinalTas.Infrastructure.Data;

public static class DataSeeder
{
    public static void SeedData(AppDbContext context)
    {
        if (context.Users.Any()) return; // Data already seeded

        // Seed Users
        var users = new List<User>
        {
            new User
            {
                Name = "Георгий Запевалов",
                Email = "zap@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Name = "Вася Пупкин",
                Email = "pup@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Seed Products
        var products = new List<Product>
        {
            new Product
            {
                Name = "iPhone 15",
                Price = 99999.99m,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Samsung Galaxy S24",
                Price = 89999.99m,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "MacBook Pro",
                Price = 199999.99m,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Dell XPS 13",
                Price = 149999.99m,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "AirPods Pro",
                Price = 24999.99m,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        // Seed Orders
        var orders = new List<Order>
        {
            new Order
            {
                UserId = users[0].Id,
                Products = new List<Product> { products[0], products[4] },
                CreatedAt = DateTime.UtcNow
            },
            new Order
            {
                UserId = users[1].Id,
                Products = new List<Product> { products[2] },
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Orders.AddRange(orders);
        context.SaveChanges();
    }
}
