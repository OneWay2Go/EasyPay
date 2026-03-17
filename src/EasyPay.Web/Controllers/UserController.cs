using EasyPay.Application.DTOs;
using EasyPay.Application.DTOs.User;
using EasyPay.Application.Interfaces;
using EasyPay.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "BotApiKey")]
public class UserController(
        IUserService userService
    )
    : ControllerBase
{
    [HttpGet("get-all-active-users")]
    public async Task<IActionResult> GetAll()
    {
        var usersDtos = await userService.GetAllActiveUsersAsync();
        return Ok(new ApiResponse<List<UserGetDto>>().Success(usersDtos));
    }

    [HttpGet("get-active-user-by-id")]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var userDto = await userService.GetActiveUserByIdAsync(id);
        if (userDto is not null)
        {
            return Ok(new ApiResponse<UserGetDto>().Success(userDto));
        }
        else
        {
            return NotFound(new ApiResponse<User>().Error("User not found"));
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] UserAddRequestDto dto)
    {
        bool isAdded = await userService.AddAsync(dto);
        if (isAdded)
        {
            return Ok(new ApiResponse<string>().Success("User added"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Error("User not added"));
        }
    }

    [HttpPut("delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid userId)
    {
        bool isDeleted = await userService.RemoveAsync(userId);
        if (isDeleted)
        {
            return Ok(new ApiResponse<string>().Success("User deleted"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Error("User not deleted, or it is already deleted"));
        }
    }

    [HttpPut("update-email-of-active-user")]
    public async Task<IActionResult> UpdateEmail([FromBody] UserEmailUpdateDto dto)
    {
        bool isUpdated = await userService.UpdateEmailOfActiveUserAsync(dto);
        if (isUpdated)
        {
            return Ok(new ApiResponse<string>().Success("User's email updated"));
        }
        else
        {
            return BadRequest(new ApiResponse<string>().Error("User's email not updated"));
        }
    }
}