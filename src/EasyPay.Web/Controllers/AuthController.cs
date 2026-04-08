namespace EasyPay.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController
{
    [HttpPost("verify")]
    public async Task<IActionResult> Verify(){
        return Ok();
    }
}