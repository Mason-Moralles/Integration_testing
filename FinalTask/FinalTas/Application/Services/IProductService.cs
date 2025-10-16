using FinalTas.Application.DTOs;

namespace FinalTas.Application.Services;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetProductsAsync(string? filter = null);
    Task<ProductResponseDto?> GetProductByIdAsync(int id);
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductResponseDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task<ProductResponseDto?> PatchProductAsync(int id, PatchProductDto patchProductDto);
    Task<bool> DeleteProductAsync(int id);
}
