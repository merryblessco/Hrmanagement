using HRbackend.Models.EmployeeModels;

namespace HRbackend.Models.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public EmployeeDto User { get; set; }
    }
}
