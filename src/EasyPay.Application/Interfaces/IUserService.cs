using EasyPay.Application.DTOs.User;

namespace EasyPay.Application.Interfaces;

public interface IUserService
{
    Task<List<UserGetDto>> GetAllActiveUsersAsync();
    Task<UserGetDto> GetActiveUserByIdAsync(Guid userId);
    Task<bool> AddAsync(UserAddRequestDto dto);
    Task<bool> RemoveAsync(Guid userId);
    Task<bool> UpdateEmailOfActiveUserAsync(UserEmailUpdateDto dto);
}