using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinalTas.Application.DTOs;
using FinalTas.Application.Services;

namespace FinalTas.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var products = app.MapGroup("/products").WithTags("Products").RequireAuthorization();

        products.MapGet("/", async (IProductService productService, [FromQuery] string? filter) =>
        {
            var result = await productService.GetProductsAsync(filter);
            return Results.Ok(result);
        })
        .WithName("GetProducts")
        .WithSummary("Get products with optional filtering")
        .Produces<List<ProductResponseDto>>(200);

        products.MapGet("/{id:int}", async (int id, IProductService productService) =>
        {
            var product = await productService.GetProductByIdAsync(id);
            return product == null 
                ? Results.NotFound(new ErrorResponseDto("Product not found", 404))
                : Results.Ok(product);
        })
        .WithName("GetProductById")
        .WithSummary("Get product by ID")
        .Produces<ProductResponseDto>(200)
        .Produces<ErrorResponseDto>(404);

        products.MapPost("/", async ([FromBody] CreateProductDto createProductDto, IProductService productService) =>
        {
            try
            {
                var product = await productService.CreateProductAsync(createProductDto);
                return Results.Created($"/products/{product.Id}", product);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ErrorResponseDto(ex.Message, 400));
            }
        })
        .WithName("CreateProduct")
        .WithSummary("Create new product")
        .Produces<ProductResponseDto>(201)
        .Produces<ErrorResponseDto>(400);

        products.MapPatch("/{id:int}", async (int id, [FromBody] PatchProductDto patchProductDto, IProductService productService) =>
        {
            try
            {
                // Validate that at least one field is provided
                if (string.IsNullOrWhiteSpace(patchProductDto.Name) && !patchProductDto.Price.HasValue)
                {
                    return Results.BadRequest(new ErrorResponseDto("At least one field (Name or Price) must be provided", 400));
                }

                var product = await productService.PatchProductAsync(id, patchProductDto);
                return product == null 
                    ? Results.NotFound(new ErrorResponseDto("Product not found", 404))
                    : Results.Ok(product);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ErrorResponseDto(ex.Message, 400));
            }
        })
        .WithName("PatchProduct")
        .WithSummary("Частично обновить продукт")
        .WithDescription("Частично обновляет информацию о продукте. Можно передать только те поля, которые нужно изменить (Name и/или Price).\n\n**Тело запроса:** JSON с полями для обновления\n\n**Возвращает:** Обновленный продукт")
        .Produces<ProductResponseDto>(200)
        .Produces<ErrorResponseDto>(400)
        .Produces<ErrorResponseDto>(404);

        products.MapDelete("/{id:int}", async (int id, IProductService productService) =>
        {
            var result = await productService.DeleteProductAsync(id);
            return result 
                ? Results.NoContent()
                : Results.NotFound(new ErrorResponseDto("Product not found", 404));
        })
        .WithName("DeleteProduct")
        .WithSummary("Delete product")
        .Produces(204)
        .Produces<ErrorResponseDto>(404);
    }
}
