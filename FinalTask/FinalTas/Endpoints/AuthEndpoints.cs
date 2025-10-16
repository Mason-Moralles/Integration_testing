using Microsoft.AspNetCore.Mvc;
using FinalTas.Application.DTOs;
using FinalTas.Application.Services;

namespace FinalTas.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/auth").WithTags("Authentication");

        auth.MapPost("/login", async ([FromBody] LoginDto loginDto, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(loginDto);
            return result == null 
                ? Results.Unauthorized() 
                : Results.Ok(result);
        })
        .WithName("Login")
        .WithSummary("User login")
        .Produces<TokenResponseDto>(200)
        .Produces<ErrorResponseDto>(401);

        auth.MapPost("/refresh", async ([FromBody] RefreshTokenDto refreshTokenDto, IAuthService authService) =>
        {
            var result = await authService.RefreshTokenAsync(refreshTokenDto);
            return result == null 
                ? Results.Unauthorized() 
                : Results.Ok(result);
        })
        .WithName("RefreshToken")
        .WithSummary("Refresh access token")
        .Produces<TokenResponseDto>(200)
        .Produces<ErrorResponseDto>(401);

        auth.MapPost("/logout", async ([FromBody] RefreshTokenDto refreshTokenDto, IAuthService authService) =>
        {
            var result = await authService.LogoutAsync(refreshTokenDto.RefreshToken);
            return result 
                ? Results.Ok(new { message = "Logged out successfully" }) 
                : Results.BadRequest(new ErrorResponseDto("Invalid refresh token", 400));
        })
        .WithName("Logout")
        .WithSummary("User logout")
        .Produces(200)
        .Produces<ErrorResponseDto>(400);
    }
}
