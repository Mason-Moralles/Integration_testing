using Microsoft.EntityFrameworkCore;
using FinalTas.Application.DTOs;
using FinalTas.Application.Services;
using FinalTas.Domain.Entities;
using FinalTas.Infrastructure.Data;
using BCrypt.Net;

namespace FinalTas.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<UserResponseDto>> GetUsersAsync(int page, int pageSize)
    {
        var totalCount = await _context.Users.CountAsync();
        var users = await _context.Users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserResponseDto(u.Id, u.Name, u.Email, u.CreatedAt))
            .ToListAsync();

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        return new PagedResultDto<UserResponseDto>(users, page, pageSize, totalCount, totalPages);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? null : new UserResponseDto(user.Id, user.Name, user.Email, user.CreatedAt);
    }

    public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

        var user = new User
        {
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponseDto(user.Id, user.Name, user.Email, user.CreatedAt);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        // Check if email is already taken by another user
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == updateUserDto.Email && u.Id != id);
        
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already taken by another user");
        }

        user.Name = updateUserDto.Name;
        user.Email = updateUserDto.Email;

        await _context.SaveChangesAsync();

        return new UserResponseDto(user.Id, user.Name, user.Email, user.CreatedAt);
    }

    public async Task<UserResponseDto?> PatchUserAsync(int id, PatchUserDto patchUserDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        // Check if email is being updated and if it's already taken by another user
        if (!string.IsNullOrWhiteSpace(patchUserDto.Email) && patchUserDto.Email != user.Email)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == patchUserDto.Email && u.Id != id);
            
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is already taken by another user");
            }
            
            user.Email = patchUserDto.Email;
        }

        // Update name only if provided
        if (!string.IsNullOrWhiteSpace(patchUserDto.Name))
        {
            user.Name = patchUserDto.Name;
        }

        await _context.SaveChangesAsync();

        return new UserResponseDto(user.Id, user.Name, user.Email, user.CreatedAt);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
