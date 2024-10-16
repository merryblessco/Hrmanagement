using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using HRbackend.Models.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace HRbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public BaseController(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        protected async Task<Guid> GetCurrentUserId()
        {
            // Retrieve the user ID from the claims using "sub" (subject) claim
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

             // Ensure userId is not null or empty before parsing
             if (string.IsNullOrEmpty(userId))
             {
                 throw new InvalidOperationException("User ID not found in claims.");
             }

             // Parse the userId to a Guid and return
             return Guid.Parse(userId);
         }

        protected async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Log claims for debugging
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
         

            if (userId == null)
            {
                return null;
            }

            return await _userManager.FindByIdAsync(userId);
        }

        // Utility function to check if user is in a specific role
        protected async Task<bool> IsUserInRoleAsync(string role)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return false;
            }

            return await _userManager.IsInRoleAsync(user, role);
        }
    }
}
