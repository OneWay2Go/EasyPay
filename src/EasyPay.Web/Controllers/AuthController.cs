using Microsoft.AspNetCore.Mvc;

namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("verify")]
    public async Task<IActionResult> Verify(){
        return Ok();
    }
}