using FinalTas.Application.DTOs;

namespace FinalTas.Application.Services;

public interface IAuthService
{
    Task<TokenResponseDto?> LoginAsync(LoginDto loginDto);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
    Task<bool> LogoutAsync(string refreshToken);
}
