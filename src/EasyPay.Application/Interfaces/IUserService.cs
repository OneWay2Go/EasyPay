using EasyPay.Application.DTOs.User;

namespace EasyPay.Application.Interfaces;

public interface IUserService
{
    Task<bool> AddAsync(UserAddRequestDto dto);
    Task<bool> RemoveAsync(Guid userId);
    Task<bool> UpdateEmailAsync(UserEmailUpdateDto dto);
}