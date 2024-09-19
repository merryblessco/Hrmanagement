using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.Recruitment
{
    public class Onboarding
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OnboardingID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public DateTime StartDate { get; set; }
        public string OnboardingDocumentFilePath { get; set; }
        public bool Completed { get; set; }
    }
}
