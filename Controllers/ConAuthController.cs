using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace APIdi.Controllers
{
[Route("api/data")]
[ApiController]
public class DataController : ControllerBase
{
    [Authorize]
    [HttpGet("secure")]
    public IActionResult GetSecureData()
    {
        return Ok(new{massage = "Hello"});
    }

    [HttpGet("public")]
    public IActionResult GetPublicData()
    {
        return Ok(new { Message = "This is public data." });
    }
}
}
