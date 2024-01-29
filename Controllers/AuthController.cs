using Microsoft.AspNetCore.Mvc;
using AuthInterfaces;
using TokenModels;
namespace APIdi.Controllers
{
[Route("api/data")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("token")]
    public IActionResult CreateToken([FromBody] TokenRequestModel model)
{
    // ตรวจสอบ model หรือ validation ตามความเหมาะสม

    var tokenString = _authService.GenerateToken(model.BoyRow!);
    return Ok(new { Token = tokenString });
}


}
}