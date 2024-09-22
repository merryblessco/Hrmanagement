using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Enums;

namespace HRbackend.Models.Entities.Recruitment
{
    public class Applicants
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicantID { get; set; }
        [ForeignKey("JobID")]
        public int JobID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ResumeFilePath { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime DOB { get; set; }
        public ApplicationStatus Status { get; set; }// (e.g., applied, shortlisted, rejected, hired)
        public string Coverletter { get; set; }
    }
}
