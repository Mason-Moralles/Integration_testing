using Microsoft.EntityFrameworkCore;
using FinalTas.Application.DTOs;
using FinalTas.Application.Services;
using FinalTas.Domain.Entities;
using FinalTas.Infrastructure.Data;
using BCrypt.Net;

namespace FinalTas.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(AppDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;

        var accessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new TokenResponseDto(accessToken, refreshToken);
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken && 
                                     !rt.IsRevoked && 
                                     rt.ExpiresAt > DateTime.UtcNow);

        if (refreshToken == null)
            return null;

        var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.User);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        refreshToken.IsRevoked = true;

        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = refreshToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return new TokenResponseDto(newAccessToken, newRefreshToken);
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null)
            return false;

        token.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
