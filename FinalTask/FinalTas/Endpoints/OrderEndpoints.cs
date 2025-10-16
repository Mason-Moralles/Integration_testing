using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinalTas.Application.DTOs;
using FinalTas.Application.Services;

namespace FinalTas.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var orders = app.MapGroup("/orders").WithTags("Orders").RequireAuthorization();

        orders.MapPost("/", async ([FromBody] CreateOrderDto createOrderDto, IOrderService orderService) =>
        {
            try
            {
                var order = await orderService.CreateOrderAsync(createOrderDto);
                return Results.Created($"/orders/{order.Id}", order);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ErrorResponseDto(ex.Message, 400));
            }
        })
        .WithName("CreateOrder")
        .WithSummary("Create new order")
        .Produces<OrderResponseDto>(201)
        .Produces<ErrorResponseDto>(400);
    }
}
