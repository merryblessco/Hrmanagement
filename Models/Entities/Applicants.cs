using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class Applicants
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicantID { get; set; }
        [ForeignKey("JobID")]
        public int JobID { get; set; }
        public string  FirstName  { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ResumeFilePath { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; }// (e.g., applied, shortlisted, rejected, hired)
    }
}
