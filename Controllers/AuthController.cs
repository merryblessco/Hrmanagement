using HRbackend.Data;
using HRbackend.Models.Auth;
using HRbackend.Models.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HRbackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;


        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
        }

        //[Authorize(Roles = "Administrator, HrManager")]
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] RegisterDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = ApplicationRoles.HrManager // Use HrApplicationRoles from enum
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Assign role to user
            await _userManager.AddToRoleAsync(user, ApplicationRoles.HrManager.ToString());

            return Ok("User created successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid credentials");
            }

            // Generate JWT access token
            var token = GenerateJwtToken(user);

            // Generate refresh token
            var refreshToken = GenerateRefreshToken();

            // Optionally, store the refresh token in the user object (if using DB storage)
            user.RefreshToken = refreshToken.Token;
            user.RefreshTokenExpiryTime = refreshToken.Expires;
            await _userManager.UpdateAsync(user);

            EmployeeContract contract = new EmployeeContract();

            var role = await _userManager.GetRolesAsync(user);

            if (role.Contains(ApplicationRoles.Employee.GetDescription()))
            {
                var employee = await _dbContext.Employees.Where(x => x.UserId == Guid.Parse(user.Id)).FirstOrDefaultAsync();

                if (employee == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                var employeeContract = await _dbContext.EmployeeContracts.Where(x => x.EmployeeId == employee.Id).FirstOrDefaultAsync();

                if (employeeContract == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                contract = employeeContract;

            }


            // Return user details, access token, and refresh token
            return Ok(new
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                User = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.GetDescription(),
                    InitialSetup = user.InitialSetup,
                    PasswordChangedStatus = user.PasswordChangedStatus,
                    IsOnboardingComplete = contract?.IsOnboardingComplete
                }
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //// Ensure new password and confirm password match
            //if (model.NewPassword != model.ConfirmNewPassword)
            //{
            //    return BadRequest(new { message = "New passwords do not match." });
            //}

            // Get the currently authenticated user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Try to change the password
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            user.PasswordChangedStatus = true;

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid email");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // In production, send this token via email to the user
            return Ok(new { Token = token });
        }

        [HttpPost("confirm-reset-password")]
        public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Invalid email");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Password reset successfully");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto tokenRequest)
        {
            // Validate the access token and refresh token
            var principal = GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token or refresh token");
            }

            var userEmail = principal.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid refresh token or refresh token expired");
            }

            // Generate new tokens
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Update user's refresh token
            user.RefreshToken = newRefreshToken.Token;
            user.RefreshTokenExpiryTime = newRefreshToken.Expires;
            await _userManager.UpdateAsync(user);

            // Return the new tokens
            return Ok(new
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Here we are checking expired tokens
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {

                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"])),
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }

        //    private string GenerateJwtToken(ApplicationUser user)
        //    {
        //        // Validate configuration settings
        //        var jwtKey = _configuration["Jwt:Key"];
        //        var jwtIssuer = _configuration["Jwt:Issuer"];
        //        var jwtAudience = _configuration["Jwt:Audience"];
        //        var expireMinutes = _configuration["Jwt:ExpireMinutes"];

        //        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) ||
        //            string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(expireMinutes))
        //        {
        //            throw new InvalidOperationException("JWT configuration is not set properly.");
        //        }

        //        var claims = new[]
        //        {
        //            new Claim(ClaimTypes.NameIdentifier, user.Id),
        //    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        //    new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //    // Optionally add roles or other claims if necessary
        //};

        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            Subject = new ClaimsIdentity(claims),
        //            Expires = DateTime.UtcNow.AddHours(1),
        //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey")), SecurityAlgorithms.HmacSha256Signature)
        //        };

        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var token = tokenHandler.CreateToken(tokenDescriptor);
        //        return tokenHandler.WriteToken(token);

        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        // Set expiration to 24 hours
        //        var expirationTime = TimeSpan.FromHours(24);

        //        var token = new JwtSecurityToken(
        //            issuer: jwtIssuer,
        //            audience: jwtAudience,
        //            claims: claims,
        //            expires: DateTime.UtcNow.Add(expirationTime),
        //            signingCredentials: creds);

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }


        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7), // Set refresh token expiration (e.g., 7 days)
                Created = DateTime.UtcNow,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            };

            return refreshToken;
        }

    }
}
