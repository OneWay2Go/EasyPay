using EasyPay.Application.DTOs.User;

namespace EasyPay.Application.Interfaces;

public interface IUserService
{
    Task<bool> AddAsync(UserRequestDto dto);
    Task<bool> RemoveAsync(Guid userId);
    Task<bool> Update(UserRequestDto dto);
}