using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

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

        public int OfferLetterStatus { get; set; } // e.g., Sent, Accepted
        public int WelcomeEmailStatus { get; set; } // e.g., Sent, Pending
        public int PaperworkStatus { get; set; } // e.g., Completed, Pending
        public int EquipmentStatus { get; set; } // e.g., Prepared, In-progress
       
        public DateTime CreatedDate { get; set; }

    }
}
