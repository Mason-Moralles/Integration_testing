using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinalTas.Application.DTOs;
using FinalTas.Application.Services;

namespace FinalTas.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var users = app.MapGroup("/users").WithTags("Users").RequireAuthorization();

        users.MapGet("/", async (IUserService userService, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        {
            var result = await userService.GetUsersAsync(page, pageSize);
            return Results.Ok(result);
        })
        .WithName("GetUsers")
        .WithSummary("Get paginated list of users")
        .Produces<PagedResultDto<UserResponseDto>>(200);

        users.MapGet("/{id:int}", async (int id, IUserService userService) =>
        {
            var user = await userService.GetUserByIdAsync(id);
            return user == null 
                ? Results.NotFound(new ErrorResponseDto("User not found", 404))
                : Results.Ok(user);
        })
        .WithName("GetUserById")
        .WithSummary("Get user by ID")
        .Produces<UserResponseDto>(200)
        .Produces<ErrorResponseDto>(404);

        users.MapPost("/", async ([FromBody] CreateUserDto createUserDto, IUserService userService) =>
        {
            try
            {
                var user = await userService.CreateUserAsync(createUserDto);
                return Results.Created($"/users/{user.Id}", user);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ErrorResponseDto(ex.Message, 400));
            }
        })
        .WithName("CreateUser")
        .WithSummary("Create new user")
        .Produces<UserResponseDto>(201)
        .Produces<ErrorResponseDto>(400);

        users.MapPatch("/{id:int}", async (int id, [FromBody] PatchUserDto patchUserDto, IUserService userService) =>
        {
            try
            {
                // Validate that at least one field is provided
                if (string.IsNullOrWhiteSpace(patchUserDto.Name) && string.IsNullOrWhiteSpace(patchUserDto.Email))
                {
                    return Results.BadRequest(new ErrorResponseDto("At least one field (Name or Email) must be provided", 400));
                }

                var user = await userService.PatchUserAsync(id, patchUserDto);
                return user == null 
                    ? Results.NotFound(new ErrorResponseDto("User not found", 404))
                    : Results.Ok(user);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ErrorResponseDto(ex.Message, 400));
            }
        })
        .WithName("PatchUser")
        .WithSummary("Частично обновить пользователя")
        .WithDescription("Частично обновляет информацию о пользователе. Можно передать только те поля, которые нужно изменить (Name и/или Email).\n\n**Тело запроса:** JSON с полями для обновления\n\n**Возвращает:** Обновленный пользователь")
        .Produces<UserResponseDto>(200)
        .Produces<ErrorResponseDto>(400)
        .Produces<ErrorResponseDto>(404);

        users.MapDelete("/{id:int}", async (int id, IUserService userService) =>
        {
            var result = await userService.DeleteUserAsync(id);
            return result 
                ? Results.NoContent()
                : Results.NotFound(new ErrorResponseDto("User not found", 404));
        })
        .WithName("DeleteUser")
        .WithSummary("Delete user")
        .Produces(204)
        .Produces<ErrorResponseDto>(404);

        users.MapGet("/{id:int}/orders", async (int id, IOrderService orderService) =>
        {
            var orders = await orderService.GetUserOrdersAsync(id);
            return Results.Ok(orders);
        })
        .WithName("GetUserOrders")
        .WithSummary("Get all orders for a user")
        .Produces<List<OrderResponseDto>>(200);
    }
}
