using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Product;

namespace APIdi.Controllers
{

    [Route("api/Product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // POST: Product/Create
        [HttpPost("AddProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ProductCD product)
        {
            // Check if the request has a valid token
            if (!User.Identity!.IsAuthenticated)
            {
                return Unauthorized(); // No valid token
            }

            // Extract roles from the user's claims
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            // Check if the "Admin" role is present in the roles
            if (!roles.Contains("Admin"))
            {
                return Forbid(); // User is not authorized
            }

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return Ok(product);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("GetProducts")]
        [Authorize] // Requires a valid token for authorization
        public async Task<ActionResult<IEnumerable<ProductCD>>> GetProducts()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); // Unauthorized if token is missing
            }

            var (isValidToken, roles) = ValidateToken(token);

            if (!isValidToken)
            {
                return Forbid(); // Forbid access if the token is invalid
            }

            var products = await _context.Products.ToListAsync();

            // Filter the products based on the user's roles
            if (roles.Contains("Admin"))
            {
                return Ok(products); // Admin sees all products
            }
            else if (roles.Contains("User"))
            {
                var filteredProducts = products.Select(p => new
                {
                    p.Name,
                    p.Price
                });

                return Ok(filteredProducts);
            }
            else
            {
                return Forbid();
            }
        }

        private (bool, List<string>) ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ACDt1vR3lXToPQ1g3MyN1234567890123456"));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = "http://127.0.0.1:5062/", // Change to your actual issuer
                    ValidateAudience = true,
                    ValidAudience = "http://127.0.0.1:5062/", // Change to your actual audience
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);


                var roles = principal.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();


                return (true, roles);
            }
            catch (Exception)
            {

                return (false, new List<string>());
            }
        }


        [HttpPut("EditProduct/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductCD updatedProduct)
        {
            // Check if the request has a valid token
            if (!User.Identity!.IsAuthenticated)
            {
                return Unauthorized(); // No valid token
            }

            // Extract roles from the user's claims
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            // Check if the "Admin" role is present in the roles
            if (!roles.Contains("Admin"))
            {
                return Forbid("No"); // User is not authorized
            }

            if (id != updatedProduct.Id)
            {
                return BadRequest("Invalid product ID");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound("Product not found");
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
                throw; // Handle concurrency conflicts if necessary
            }

            return Ok(existingProduct);
        }

        [HttpDelete("DeleteProduct/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            // Check if the request has a valid token
            if (!User.Identity!.IsAuthenticated)
            {
                return Unauthorized(); // No valid token
            }

            // Extract roles from the user's claims
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            // Check if the "Admin" role is present in the roles
            if (!roles.Contains("Admin"))
            {
                return Forbid(); // User is not authorized
            }

            // Continue with product deletion logic
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Product not found");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Product deleted successfully");
        }





    }
}