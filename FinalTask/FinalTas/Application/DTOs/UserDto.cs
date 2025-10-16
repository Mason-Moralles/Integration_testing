namespace FinalTas.Application.DTOs;

public record CreateUserDto(string Name, string Email, string Password);
public record UpdateUserDto(string Name, string Email);
public record PatchUserDto(string? Name, string? Email);
public record UserResponseDto(int Id, string Name, string Email, DateTime CreatedAt);
public record LoginDto(string Email, string Password);
public record TokenResponseDto(string AccessToken, string RefreshToken);
public record RefreshTokenDto(string RefreshToken);
