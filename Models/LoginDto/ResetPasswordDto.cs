namespace HRbackend.Models.LoginDto
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string AccessToken { get; set; }
    }
}
