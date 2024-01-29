
using LoginModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using InterfaceAccountService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Product;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace AccountService;
public class accountService : IAccountService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly ApplicationDbContext _context;


    public accountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITempDataDictionaryFactory tempDataDictionaryFactory, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
        _context = context;
        _roleManager = roleManager;
    }
    public async Task<IEnumerable<ProductCD>> GetProducts()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<(ProductCD, ValidationResult)> AddProduct(ProductCD product)
    {
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(product, new ValidationContext(product), validationResults, true);

        if (isValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return (product, null!);
        }

        return (null!, new ValidationResult("Model validation failed"));
    }

    public async Task<(ProductCD, ValidationResult)> UpdateProduct(int id, ProductCD updatedProduct)
    {
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(updatedProduct, new ValidationContext(updatedProduct), validationResults, true);

        if (!isValid)
        {
            return (null!, new ValidationResult("Model validation failed"));
        }

        var existingProduct = await _context.Products.FindAsync(id);

        if (existingProduct == null)
        {
            return (null!, new ValidationResult("Product not found"));
        }

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Price = updatedProduct.Price;
        // Update other properties as needed

        try
        {
            _context.Entry(existingProduct).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return (null!, new ValidationResult("Concurrency conflict"));
        }

        return (existingProduct, null!);
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return false; // Product not found
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email!);

        if (existingUser != null)
        {
            return new ConflictObjectResult($"User with email {model.Email!} already exists");
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            // ... (โค้ดที่มีอยู่ในการตรวจสอบและเพิ่มบทบาท)

            return new BadRequestObjectResult("User registration failed");
        }

        // เพิ่มบทบาทจากข้อมูลที่ส่งมา
        if (!string.IsNullOrEmpty(model.Role))
        {
            // ตรวจสอบว่าบทบาทที่ส่งมาอยู่ใน AspNetRoles
            var roleExists = await _roleManager.RoleExistsAsync(model.Role);

            if (roleExists)
            {
                // เพิ่มผู้ใช้ในบทบาทที่ระบุ
                await _userManager.AddToRoleAsync(user, model.Role);
            }
            else
            {
                return new BadRequestObjectResult($"Role {model.Role} does not exist");
            }
        }

        return new OkObjectResult("User registered successfully");
    }

    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(model.Email!, model.Password!, isPersistent: false, lockoutOnFailure: false);

        if (signInResult.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);
            var userRoles = await _userManager.GetRolesAsync(user!);

            if (userRoles.Any())
            {
                Console.WriteLine($"User {user!.UserName} has roles: {string.Join(", ", userRoles)}");

                var token = await GenerateToken(model.Email!, userRoles.ToList());

                return new ObjectResult(new
                {
                    Message = "User logged in successfully",
                    Token = token,
                    Roles = userRoles
                });
            }
            else
            {
                Console.WriteLine($"User {user!.UserName} has no roles");

                var token = await GenerateToken(model.Email!, new List<string>());

                return new ObjectResult(new
                {
                    Message = "User logged in successfully",
                    Token = token,
                    Roles = new List<string>()
                });
            }
        }
        else
        {
            return new BadRequestObjectResult("Login failed");
        }
    }


    public async Task<IActionResult> Logout(HttpContext httpContext)
    {
        var user = httpContext.User;
        if (user.Identity!.IsAuthenticated)
        {
            await _signInManager.SignOutAsync();
            return new OkObjectResult(new { Message = "Logout success" });
        }
        else
        {
            return new BadRequestObjectResult(new { Message = "User is not logged in" });
        }
    }

    public IActionResult GetAllUsers()
    {
        var users = _userManager.Users.ToList();
        return new OkObjectResult(users);
    }

    public async Task<IActionResult> Logout()
    {
        // Check if the user is authenticated
        var user = _signInManager.Context.User;
        if (user.Identity!.IsAuthenticated)
        {
            // Perform the logout
            await _signInManager.SignOutAsync();
            return new OkObjectResult(new { Message = "Logout success" });
        }
        else
        {
            return new BadRequestObjectResult(new { Message = "User is not logged in" });
        }
    }
    public async Task<IActionResult> ChangePassword(string userId, ChangePasswordViewModel model)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return new BadRequestObjectResult("User not found");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword!, model.NewPassword!);

        if (result.Succeeded)
        {
            // If the password change is successful, you may want to sign the user out
            await _signInManager.SignOutAsync();
            return new OkObjectResult("Password changed successfully. Please log in with the new password.");
        }
        else
        {
            // Handle password change failure
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error Code: {error.Code}, Description: {error.Description}");
                // Handle specific error cases if needed
            }

            return new BadRequestObjectResult("Password change failed");
        }
    }
    public async Task<string> GenerateToken(string email, List<string> roles)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new ApplicationException($"User with email {email} not found");
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Email, email)
    };

        // เพิ่ม claims สำหรับ roles ที่ user มี
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ACDt1vR3lXToPQ1g3MyN1234567890123456")); // ใส่ Secret Key ของคุณที่นี่
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "http://127.0.0.1:5062/", // ใส่ Issuer ของคุณที่นี่
            audience: "http://127.0.0.1:5062/", // ใส่ Audience ของคุณที่นี่
            claims: claims,
            expires: DateTime.Now.AddHours(1), // กำหนดเวลาหมดอายุของ token ที่นี่
            signingCredentials: creds
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
    

}