using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.PayRoll
{
    public class BenefitAdministration : BaseEntity
    {
        public Guid EmployeeID { get; set; }
        public Guid BenefitType { get; set; } // e.g., Health Insurance, Retirement Plan
        public decimal BenefitAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
