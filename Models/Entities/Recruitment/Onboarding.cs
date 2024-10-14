using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities.Recruitment
{
    public class Onboarding : BaseEntity
    {
        public Guid EmployeeID { get; set; }

        public DateTime ResumptionDate { get; set; }

        public int OfferLetterStatus { get; set; } // e.g., Sent, Accepted
        public int WelcomeEmailStatus { get; set; } // e.g., Sent, Pending
        public int PaperworkStatus { get; set; } // e.g., Completed, Pending
        public int EquipmentStatus { get; set; } // e.g., Prepared, In-progress
       
        public DateTime CreatedDate { get; set; }
        public Employee Employee { get; set; }

    }
}
