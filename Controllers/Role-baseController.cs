using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIdi.Controllers
{
    [Route("api/authorization")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // private readonly UserManager<IdentityUser> _userManager;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
            // _userManager = userManager;
        }
        // [Authorize(Roles = Roles.Admin)]

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult SomeActionForAdmin()
        {
            // var user = await _userManager.FindByEmailAsync(email);
            // Console.WriteLine("H"+user);
            // Code for Adminsur
            return Ok("Admin action successful");
        }
        [Authorize(Policy = "UserOnly")]
        [HttpGet("User")]
        public IActionResult SomeActionForUser()
        {
            // Code for User
            return Ok("User action successful");
        }

        [HttpGet("common-action")]
        public IActionResult CommonAction()
        {
            // Common code for all users
            return Ok("Common action successful");
        }


        // [Authorize(Policy = "AdminOnly")]
        // [HttpPost]
        // public async Task<IActionResult> CreateUser([FromBody] User userInput)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Users.Add(userInput);
        //         await _context.SaveChangesAsync();
        //         return Ok("User created successfully");
        //     }

        //     return BadRequest("Invalid model state");
        // }

        // [HttpGet("GetUser")]
        // public IActionResult GetUsers()
        // {
        //     var users = _context.User.ToList();
        //     return Ok(users);
        // }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteUser(int id)
        // {
        //     var user = await _context.User.FindAsync(id);

        //     if (user == null)
        //     {
        //         return NotFound("User not found");
        //     }

        //     _context.User.Remove(user);
        //     await _context.SaveChangesAsync();

        //     return Ok("User deleted successfully");
        // }

        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        // {
        //     var user = await _context.User.FindAsync(id);

        //     if (user == null)
        //     {
        //         return NotFound("User not found");
        //     }

        //     // อัปเดตข้อมูลผู้ใช้
        //     user.Username = updatedUser.Username;
        //     // ทำการอัปเดตข้อมูลอื่น ๆ ตามต้องการ

        //     await _context.SaveChangesAsync();

        //     return Ok("User updated successfully");
        // }

    }

}