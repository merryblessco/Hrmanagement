using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.Entities;
using LinkOrgNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HRbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AdminAuthController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IMapper mapper)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }
        // Admin sign-up
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] Admin admin)
        {
            if (await _dbContext.Admins.AnyAsync(a => a.Email == admin.Email))
            {
                return BadRequest(new { message = "Admin already exists" });
            }

            // Hash password before saving to DB
            admin.PasswordHash = SecurityClass.FCODE(admin.PasswordHash);

            _dbContext.Admins.Add(admin);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Admin registered successfully" });
        }
        // Admin login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Admin admin)
        {
            var existingAdmin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == admin.Email);

            if (existingAdmin == null)
            {
                return BadRequest(new { message = "Invalid credentials" });
            }

            // Generate JWT token
            var token = GenerateJwtToken(existingAdmin);

            return Ok(new { token });
        }
        
        private string GenerateJwtToken(Admin admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtConfig:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                    new Claim(ClaimTypes.Email, admin.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
