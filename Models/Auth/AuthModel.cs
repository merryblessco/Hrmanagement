using HRbackend.Models.Enums;

namespace HRbackend.Models.Auth
{
    public class RegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; }
    }

    public class ConfirmResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class ChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }


}
