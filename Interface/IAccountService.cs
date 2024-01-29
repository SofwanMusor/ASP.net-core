using LoginModel;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Product;
using System.ComponentModel.DataAnnotations;

namespace InterfaceAccountService;
public interface IAccountService
{
    Task<IActionResult> Register(RegisterViewModel model);
    Task<IActionResult> Login(LoginViewModel model);
    Task<IActionResult> Logout();
    IActionResult GetAllUsers();
    Task<IActionResult> ChangePassword(string userId, ChangePasswordViewModel model);
}
