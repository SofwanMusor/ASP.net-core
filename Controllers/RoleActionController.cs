using Microsoft.AspNetCore.Mvc;
using InterfaceRole;
using LoginModel;

namespace APIdi.Controllers
{
    [Route("api/Role")]
    [ApiController]
    public class RolesController : Controller
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Subdata model)
        {
            if (ModelState.IsValid)
            {
                await _rolesService.CreateRoleAsync(model.RoleName!);
                return Ok();
            }
            return BadRequest(ModelState);
        }

        // ลบส่วนที่ไม่เกี่ยวข้องกับการเพิ่ม Role ทั้งหมด
    }
}
