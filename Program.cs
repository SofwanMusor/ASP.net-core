using AuthServices;
using AuthInterfaces;
using FileInterfaces;
using FileServices;
using UserServices;
using InsertUser;
using UserInterfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DataRoles;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using InterfaceAccountService;
using AccountService;
using InterfaceRole;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMvc();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IinsertUserService, InsertUserService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRolesService, RolesService>();
builder.Services.AddScoped<IAccountService, accountService>();
builder.Logging.AddDebug();

builder.Logging.SetMinimumLevel(LogLevel.Debug);
var connectionString = "Server=localhost;Database=User;User=root;Password=123456";
// Configure your database connection using the connectionString

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true, // เพิ่มการตรวจสอบ Issuer
            ValidateAudience = true, // เพิ่มการตรวจสอบ Audience
            ClockSkew = TimeSpan.Zero, // ไม่ให้มีการปรับเวลา
            ValidIssuer = "http://127.0.0.1:5062/",
            ValidAudience = "http://127.0.0.1:5062/",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ACDt1vR3lXToPQ1g3MyN1234567890123456"))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy("UserOnly", policy => policy.RequireRole(Roles.User));
});
Console.WriteLine(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), opt => opt.EnableRetryOnFailure()));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(Options =>{
    Options.User.RequireUniqueEmail = true;
    Options.SignIn.RequireConfirmedAccount = false;
    Options.Password.RequireDigit = false;
    Options.Password.RequireLowercase = false;
    Options.Password.RequireUppercase = false; // ยังต้องการตัวใหญ่
    Options.Password.RequireNonAlphanumeric = false;
    Options.Password.RequiredLength = 8; // ความยาวขั้นต่ำ
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
    
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDeveloperExceptionPage();
app.UseApiKeyMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
