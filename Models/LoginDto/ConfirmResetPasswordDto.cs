namespace HRbackend.Models.LoginDto
{
    public class ConfirmResetPasswordDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
        
        public string ResetToken { get; set; }
    }
}
