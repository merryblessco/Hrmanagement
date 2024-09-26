using AutoMapper;
using HRbackend.Data;
using HRbackend.Models.Auth;
using HRbackend.Models.EmployeeModels;
using HRbackend.Models.Entities.Employees;
using HRbackend.Models.LoginDto;
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
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public AuthController(ApplicationDbContext dbContext, IWebHostEnvironment environment, IConfiguration configuration, IMapper mapper)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Email == model.Email);

            //if (employee == null || SecurityClass.FCODE(model.Password))
            //{
            //    return Unauthorized("Invalid email or password");
            //}

            var mappedEmployee = _mapper.Map<EmployeeDto>(employee);
            var token = GenerateJwtToken(employee, "access");
            var accessToken = GenerateJwtToken(employee, "access");
            var refreshToken = GenerateJwtToken(employee, "refresh");

            var res = new LoginResponseDto()
            {
                User = mappedEmployee,
                Token = token,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return Ok(res);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var principal = GetPrincipalFromExpiredToken(model.RefreshToken);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Email == email);

            if (employee == null)
            {
                return BadRequest("Invalid refresh token");
            }

            var newAccessToken = GenerateJwtToken(employee, "access");
            var newRefreshToken = GenerateJwtToken(employee, "refresh");

            return Ok(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Email == model.Email);

            if (employee == null)
            {
                return BadRequest("Invalid email");
            }

            // Generate a password reset token
            var resetToken = GenerateJwtToken(employee, "reset");

            // In a real-world scenario, you would send this token to the user's email
            // For this example, we'll just return it
            return Ok(new { ResetToken = resetToken });
        }

        [HttpPost("confirm-reset-password")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmResetPasswordDto model)
        {
            var principal = GetPrincipalFromExpiredToken(model.ResetToken);
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Email == email);

            if (employee == null)
            {
                return BadRequest("Invalid reset token");
            }

            employee.Password = SecurityClass.FCODE(model.NewPassword);
            await _dbContext.SaveChangesAsync();

            return Ok("Password reset successfully");
        }
        private string GenerateJwtToken(Employee employee, string tokenType)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("JWT configuration is incomplete. Please check your appsettings.json file.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.EmployeeID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, employee.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("token_type", tokenType)
            };

            var expirationTime = tokenType switch
            {
                "access" => DateTime.Now.AddMinutes(15),
                "refresh" => DateTime.Now.AddDays(7),
                "reset" => DateTime.Now.AddHours(1),
                _ => throw new ArgumentException("Invalid token type")
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expirationTime,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
    /*private string GenerateJwtToken(Employee employee)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, employee.EmployeeID.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, employee.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }*/

}

