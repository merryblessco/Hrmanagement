using HRbackend.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace HRbackend.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        // Automatically set Fullname on initialization
        public string FullName => $"{FirstName} {LastName}";

        public ApplicationRoles Role { get; set; }

        public bool InitialSetup { get; set; } = false;
        public bool PasswordChangedStatus { get; set; } = false;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime? RefreshTokenExpiryTime { get; set; }

    }
}
