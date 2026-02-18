using EasyPay.Application.DTOs.User;
using EasyPay.Application.Interfaces;
using EasyPay.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
    EPContext context,
    IUserService userService
    )
    : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var users = await context.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] Guid id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        if (user is not null)
        {
            return Ok(user);
        }
        else
        {
            return NotFound("User not found");
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] UserRequestDto dto)
    {
        bool isAdded = await userService.AddAsync(dto);
        if (isAdded)
        {
            return Ok("User added");
        }
        else
        {
            return BadRequest("User not added, and I dont know why");
        }
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromQuery] Guid userId)
    {
        bool isDeleted = await userService.RemoveAsync(userId);
        if (isDeleted)
        {
            return Ok("User deleted");
        }
        else
        {
            return BadRequest("User not deleted, and I think, I do know why");
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UserRequestDto dto)
    {
        bool isUpdated = await userService.Update(dto);
        if (isUpdated)
        {
            return Ok("User updated");
        }
        else
        {
            return BadRequest("User not updated, and I dont know why");
        }
    }
}