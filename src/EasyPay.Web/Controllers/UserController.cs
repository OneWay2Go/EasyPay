using EasyPay.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(EPContext context)
    : ControllerBase
{
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var users = await context.Users.ToListAsync();
        return Ok(users);
    }
}