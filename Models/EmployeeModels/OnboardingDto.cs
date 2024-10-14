using HRbackend.Models.Entities.Employees;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class OnboardingDto
    {
       
        public Guid EmployeeID { get; set; }
        public DateTime ResumptionDate { get; set; }
        //public string OnboardingDocumentFilePath { get; set; }
        public int? OfferLetterStatus { get; set; } // e.g., Sent, Accepted
        public int? WelcomeEmailStatus { get; set; } // e.g., Sent, Pending
        public int? PaperworkStatus { get; set; } // e.g., Completed, Pending
        public int? EquipmentStatus { get; set; } // e.g., Prepared, In-progress

        public DateTime CreatedDate { get; set; }

    }
}
