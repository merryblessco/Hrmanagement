using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.AdminModel
{
    public class AdminDto
    {
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
