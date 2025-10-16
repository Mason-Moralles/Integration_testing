namespace FinalTas.Application.DTOs;

public record CreateProductDto(string Name, decimal Price);
public record UpdateProductDto(string Name, decimal Price);
public record PatchProductDto(string? Name, decimal? Price);
public record ProductResponseDto(int Id, string Name, decimal Price, DateTime CreatedAt);
