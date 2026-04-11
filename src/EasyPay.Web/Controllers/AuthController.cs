using EasyPay.Application.DTOs.Auth;
using EasyPay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody]AuthLoginDto dto)
    {
        string token = await authService.LoginAsync(dto.Username, dto.Password);

        return Ok(token);
    }
}   