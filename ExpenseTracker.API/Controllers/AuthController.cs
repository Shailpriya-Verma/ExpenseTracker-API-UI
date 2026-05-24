using ExpenseTracker.API.Data;
using ExpenseTracker.API.DTOs.Auth;
using ExpenseTracker.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ExpenseTracker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IConfiguration configuration;
        public AuthController(AppDbContext _dbContext,IConfiguration configuration)
        {
            dbContext = _dbContext;
            this.configuration = configuration;
        }

        

        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool userExists = await dbContext.tbl_User.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (userExists)
                return BadRequest("User With Same Email Already Exists");

            // Hash password using BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            User user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = hashedPassword
            };
            dbContext.tbl_User.Add(user);
            await dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Registered Successfully" });
        }
        #endregion Register


        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await dbContext.tbl_User.FirstOrDefaultAsync(
                u=> u.Email.ToLower()== dto.Email.ToLower()
            );

            if (user == null)
                return BadRequest(new{success=false,message="Invalid Email"});

            // Verify password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return BadRequest(new { success = false, message = "Invalid Password" });

            string token = GenerateJwtToken(user);

            return Ok(new
            {
                success = true,
                message = "Login successful",
                token = token
            });
        }
        #endregion Login

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("FullName", user.FullName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
