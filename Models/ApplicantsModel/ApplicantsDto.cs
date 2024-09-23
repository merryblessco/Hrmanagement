using HRbackend.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.ApplicantsModel
{
    public class ApplicantsDto
    {
        public int JobID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public string Fullname { get; set; }
        [NotMapped]
        public string Fullname => $"{FirstName} {LastName}";
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public string? StatusText { get; set; }
        public ApplicationStatus? Status { get; set; }
        public string Coverletter { get; set; }
        public DateTime ApplicationDate { get; set; }

        public IFormFile Resume { get; set; }
    }
}
