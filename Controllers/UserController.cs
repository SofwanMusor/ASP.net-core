using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserInterfaces;
using  APIdi.Models;

namespace APIdi.Controllers;

    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetUser([FromQuery] UserParam param)
        {
            var users = _userService.GetUsers(param);
            return Ok(users);
        }


        [HttpGet]
        [Route("{id}")]

        public ActionResult GetUserById([FromQuery] int id)
        {
            var users = UserStore.Users;

            var user = users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound($"user with id  {id} does not exist");
            }

            return Ok(user);
        }


        [HttpDelete]
        [Route("{id}")]
        public ActionResult RemoveUser([FromRoute] int id)
        {
            var users = UserStore.Users;

            var user = users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound($"user with id  {id} does not exist");
            }

            users.Remove(user);
            return Ok("delete user successfully");
        }


        [HttpPost]
        [Route("{id}")]

        public ActionResult EditUser([FromBody] UserModel model, [FromRoute] int id)
        {
            var users = UserStore.Users;

            var user = users.FirstOrDefault(x => x.Id == id);

            if (user == null)
            {
                return NotFound($"user with id  {id} does not exist");
            }


            UserModel newUser = new UserModel
            {
                Id = id,
                Name = model.Name

            };

            users.Remove(user);
            users.Add(newUser);

            return Ok(users);
        }
        [HttpPost]
        public ActionResult CreateUser([FromBody] UserModel model)
        {
            var users = UserStore.Users;

            var user = users.FirstOrDefault(x => x.Name == model.Name);

            if (user != null)
            {
                return Conflict($"user with id  {model.Name} already exist");
            }

            int id = users.Max(x => x.Id);
            _logger.LogInformation($"max id is {id}");

            UserModel newUser = new UserModel
            {
                Name = model.Name,
                Id = id + 1

            };
            users.Add(newUser);
            return Ok(users);
        }



    }
