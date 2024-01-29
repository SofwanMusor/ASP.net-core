using InterfaceAccountService;
using LoginModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
namespace APIdi.Controllers
{


    [ApiController]
    [Route("api/Identity")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;


        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            return await _accountService.Register(model);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            return await _accountService.Login(model);
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            return await _accountService.Logout();
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            return _accountService.GetAllUsers();
        }
        [HttpPost("ChangePassword/{userId}")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordViewModel model)
        {
            return await _accountService.ChangePassword(userId, model);
        }
        [HttpGet("Test")]
        [Authorize] // ตรวจสอบ Token ทุกรายการที่เข้าถึง Endpoint นี้
        public IActionResult GetClaimsFromToken()
        {
            try
            {
                // ดึง Token จาก Header
                var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                // แปลง Token เป็น Claims
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    // ดึง Claims จาก Token
                    var claims = jsonToken.Claims;

                    // สร้าง List เพื่อเก็บข้อมูล Claim Types และ Claim Values
                    var claimData = new List<object>();

                    // วนลูปเพื่อเก็บข้อมูล Claim Types และ Claim Values ลงใน List
                    foreach (var claim in claims)
                    {
                        claimData.Add(new { Type = claim.Type, Value = claim.Value });
                    }

                    // ส่งข้อมูล Claims กลับใน Response
                    return Ok(new { Claims = claimData });
                }

                return BadRequest(new { Message = "Invalid Token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

    }
}