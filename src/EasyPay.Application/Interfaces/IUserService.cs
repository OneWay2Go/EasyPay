using EasyPay.Application.DTOs.User;

namespace EasyPay.Application.Interfaces;

public interface IUserService
{
    Task<List<UserGetDto>> GetAllAsync();
    Task<UserGetDto> GetByIdAsync(Guid userId);
    Task<bool> AddAsync(UserAddRequestDto dto);
    Task<bool> RemoveAsync(Guid userId);
    Task<bool> UpdateEmailAsync(UserEmailUpdateDto dto);
}