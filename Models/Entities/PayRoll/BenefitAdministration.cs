using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.PayRoll
{
    public class BenefitAdministration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BenefitAdministrationId { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public int BenefitType { get; set; } // e.g., Health Insurance, Retirement Plan
        public decimal BenefitAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
