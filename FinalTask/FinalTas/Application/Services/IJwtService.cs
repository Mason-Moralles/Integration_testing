using FinalTas.Domain.Entities;

namespace FinalTas.Application.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    int? ValidateToken(string token);
}
