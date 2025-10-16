using Microsoft.EntityFrameworkCore;
using FinalTas.Application.DTOs;
using FinalTas.Application.Services;
using FinalTas.Domain.Entities;
using FinalTas.Infrastructure.Data;

namespace FinalTas.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto)
    {
        var products = await _context.Products
            .Where(p => createOrderDto.ProductIds.Contains(p.Id))
            .ToListAsync();

        var order = new Order
        {
            UserId = createOrderDto.UserId,
            Products = products,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var productDtos = products.Select(p => new ProductResponseDto(p.Id, p.Name, p.Price, p.CreatedAt)).ToList();
        return new OrderResponseDto(order.Id, order.UserId, productDtos, order.CreatedAt);
    }

    public async Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId)
    {
        var orders = await _context.Orders
            .Include(o => o.Products)
            .Where(o => o.UserId == userId)
            .ToListAsync();

        return orders.Select(o => new OrderResponseDto(
            o.Id,
            o.UserId,
            o.Products.Select(p => new ProductResponseDto(p.Id, p.Name, p.Price, p.CreatedAt)).ToList(),
            o.CreatedAt
        )).ToList();
    }
}
