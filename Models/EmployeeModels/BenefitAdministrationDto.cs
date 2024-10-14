using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class BenefitAdministrationDto
    {
        public Guid EmployeeID { get; set; }

        public Guid BenefitType { get; set; } // e.g., Health Insurance, Retirement Plan
        public decimal BenefitAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
