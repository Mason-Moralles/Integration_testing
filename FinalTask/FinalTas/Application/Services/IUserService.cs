using FinalTas.Application.DTOs;

namespace FinalTas.Application.Services;

public interface IUserService
{
    Task<PagedResultDto<UserResponseDto>> GetUsersAsync(int page, int pageSize);
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
    Task<UserResponseDto?> PatchUserAsync(int id, PatchUserDto patchUserDto);
    Task<bool> DeleteUserAsync(int id);
}
