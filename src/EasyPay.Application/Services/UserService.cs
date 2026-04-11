using EasyPay.Application.DTOs.User;
using EasyPay.Application.Interfaces;
using EasyPay.Core.Entities;
using EasyPay.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Application.Services;

public class UserService(EPContext context)
    : IUserService
{
    public async Task<List<UserGetDto>> GetAllActiveUsersAsync()
    {
        var users = await context.Users
            .Where(u => u.IsDeleted == false).ToListAsync();
        List<UserGetDto> usersDto = new List<UserGetDto>();
        foreach (var user in users)
        {
            usersDto.Add(new UserGetDto
            {
                Username = user.Username,
                WalletBalance = user.WalletBalance
            });
        }

        return usersDto;
    }

    public async Task<UserGetDto> GetActiveUserByIdAsync(Guid userId)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted == false);
        if (user is not null)
        {
            UserGetDto userDto = new UserGetDto()
            {
                Username = user.Username,
                WalletBalance = user.WalletBalance
            };
        
            return userDto;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> AddAsync(UserAddRequestDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, 13)
        };
        
        await context.Database.BeginTransactionAsync();
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            
            await context.Database.CommitTransactionAsync();
            
            return true;
        }
        catch
        {
            await context.Database.RollbackTransactionAsync();
            
            return false;
        }
    }

    public async Task<bool> RemoveAsync(Guid userId)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted == false);
        if (user is not null)
        {
            await context.Database.BeginTransactionAsync();
            try
            {
                user.IsDeleted = true;
                context.Update(user);
                await context.SaveChangesAsync();
                
                await context.Database.CommitTransactionAsync();
                
                return true;
            }
            catch
            {
                await context.Database.RollbackTransactionAsync();
                
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> UpdateEmailOfActiveUserAsync(UserEmailUpdateDto dto)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsDeleted == false);
        if (user is not null)
        {
            await context.Database.BeginTransactionAsync();
            try
            {
                // need to send confirmation link from frontend
                user.Email = dto.NewEmail;
            
                context.Users.Update(user);
                await context.SaveChangesAsync();
                
                await context.Database.CommitTransactionAsync();
                
                return true;
            }
            catch (Exception ex)
            {
                await context.Database.RollbackTransactionAsync();
                
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task<User> GetActiveUserByUsernameAsync(string username)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsDeleted == false);
        if (user is not null)
        {
            return user;
        }
        else
        {
            return null;
        }
    }
}