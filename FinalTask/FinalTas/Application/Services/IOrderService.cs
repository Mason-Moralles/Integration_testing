using FinalTas.Application.DTOs;

namespace FinalTas.Application.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto);
    Task<List<OrderResponseDto>> GetUserOrdersAsync(int userId);
}
