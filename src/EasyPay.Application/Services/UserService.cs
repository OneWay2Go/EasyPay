using EasyPay.Application.DTOs.User;
using EasyPay.Application.Interfaces;
using EasyPay.Core.Entities;
using EasyPay.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Application.Services;

public class UserService(EPContext context)
    : IUserService
{
    public async Task<bool> AddAsync(UserRequestDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, 13)
        };

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> RemoveAsync(Guid userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user is not null)
        {
            try
            {
                context.Remove(user);
                await context.SaveChangesAsync();
                
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> Update(UserRequestDto dto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user is not null)
        {
            user.Email = dto.Email;
            
            context.Users.Update(user);
            await context.SaveChangesAsync();

            return true;
        }
        else
        {
            return false;
        }
    }
}