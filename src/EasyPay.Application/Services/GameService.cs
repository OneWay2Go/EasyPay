using EasyPay.Application.DTOs.Game;
using EasyPay.Application.Interfaces;
using EasyPay.Core.Entities;
using EasyPay.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Application.Services;

public class GameService(EPContext context) : IGameService
{
    public async Task<List<GameGetDto>> GetAllAsync()
    {
        var games = await context.Games.ToListAsync();
        List<GameGetDto> gamesDto = new List<GameGetDto>();
        foreach (var game in games)
        {
            gamesDto.Add(new GameGetDto()
            {
                Name = game.Name
            });
        }
        
        return gamesDto;
    }

    public async Task<GameGetDto> GetByIdAsync(Guid gameId)
    {
        var game = await context.Games.FirstOrDefaultAsync(g => g.GameId == gameId);
        if (game is not null)
        {
            GameGetDto gameDto = new GameGetDto()
            {
                Name = game.Name
            };
            
            return gameDto;
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> AddAsync(GameAddDto dto)
    {
        var game = new Game()
        {
            Name = dto.Name
        };
        
        await context.Database.BeginTransactionAsync();
        try
        {
            await context.Games.AddAsync(game);
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

    public async Task<bool> RemoveAsync(Guid gameId)
    {
        var game = await context.Games.FirstOrDefaultAsync(g => g.GameId == gameId);
        if (game is not null)
        {
            await context.Database.BeginTransactionAsync();
            try
            {
                context.Games.Remove(game);
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
}