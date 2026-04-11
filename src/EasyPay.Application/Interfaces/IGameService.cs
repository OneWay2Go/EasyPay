using EasyPay.Application.DTOs.Game;

namespace EasyPay.Application.Interfaces;

public interface IGameService
{
    Task<List<GameGetDto>> GetAllAsync();
    Task<GameGetDto> GetByIdAsync(Guid gameId);
    Task<bool> AddAsync(GameAddDto dto);
    Task<bool> RemoveAsync(Guid gameId);
}