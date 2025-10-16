namespace FinalTas.Application.DTOs;

public record CreateOrderDto(int UserId, List<int> ProductIds);
public record OrderResponseDto(int Id, int UserId, List<ProductResponseDto> Products, DateTime CreatedAt);
