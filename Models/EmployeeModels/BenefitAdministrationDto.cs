using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class BenefitAdministrationDto
    {
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public int BenefitType { get; set; } // e.g., Health Insurance, Retirement Plan
        public decimal BenefitAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
