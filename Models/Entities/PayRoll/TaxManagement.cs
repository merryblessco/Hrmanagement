using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.PayRoll
{
    public class TaxManagement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaxManagementId { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal SocialSecurityTax { get; set; }
        public decimal MedicareTax { get; set; }
        public DateTime TaxYear { get; set; }
    }
}
