using Microsoft.AspNetCore.Mvc;
using APIdi.Models;
namespace UserInterfaces
{
    public interface IUserService
    {
        public ActionResult GetUsers (UserParam param);

        public Task<ActionResult> CreatUser (UserModel model);
    }
}