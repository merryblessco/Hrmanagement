namespace HRbackend.Models.LoginDto
{
    public class AdminSignupDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } = "Admin"; // Default role is Admin
    }
    public class ConfirmResetPasswordDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        
        public string ResetToken { get; set; }
    }
}
