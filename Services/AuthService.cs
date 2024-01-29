using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthInterfaces;

namespace AuthServices;
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    // private readonly ApplicationDbContext _dbContext;

    public AuthService(IConfiguration configuration)
    // public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
        // _dbContext = dbContext;
    }

    public string GenerateToken(string boyRow)
    {
        // bool userExists = _dbContext.IsUserExists(boyRow);
        // if (userExists)
        // {
            var key = Encoding.UTF8.GetBytes("ACDt1vR3lXToPQ1g3MyN1234567890123456");

#pragma warning disable SYSLIB0021 // Type or member is obsolete
        using (var sha256 = new System.Security.Cryptography.SHA256Managed())
        {
            key = sha256.ComputeHash(key);
        }
#pragma warning restore SYSLIB0021 // Type or member is obsolete

        var tokenHandler = new JwtSecurityTokenHandler();
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ACDt1vR3lXToPQ1g3MyN1234567890123456"));

        var tokenDescriptor = new JwtSecurityToken(
            issuer: "http://127.0.0.1:5062/",
            audience: "http://127.0.0.1:5062/",
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            claims: new List<Claim>
            {
            new Claim(ClaimTypes.Role, boyRow), // ใช้ค่าจาก boyRow เป็น Role
                                                // เพิ่ม claims อื่น ๆ ตามต้องการ
            }
        );

        var tokenString = tokenHandler.WriteToken(tokenDescriptor);

        return tokenString;
        }

    public Task<bool> InsertUserAsync(string username)
    {
        throw new NotImplementedException();
    }
    // else
    // {
    //     return null!;
    // }


}
