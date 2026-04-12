using EasyPay.Application.DTOs;
using EasyPay.Application.DTOs.Game;
using EasyPay.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController(IGameService gameService) : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var gamesDto = await gameService.GetAllAsync();
        return Ok(new ApiResponse<List<GameGetDto>>().Success(gamesDto));
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] Guid gameId)
    {
        var gameDto = await gameService.GetByIdAsync(gameId);
        if (gameDto is not null)
        {
            return Ok(new ApiResponse<GameGetDto>().Success(gameDto));
        }
        else
        {
            return NotFound(new ApiResponse<string>().Error("Game not found"));
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] GameAddDto gameAddDto)
    {
        bool isAdded = await gameService.AddAsync(gameAddDto);
        if (isAdded)
        {
            return Ok(new ApiResponse<string>().Success("Game added"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Error("Game not added"));
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid gameId)
    {
        bool isDeleted = await gameService.RemoveAsync(gameId);
        if (isDeleted)
        {
            return Ok(new ApiResponse<string>().Success("Game deleted"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Error("Game not deleted, or it is already deleted"));
        }
    }
}