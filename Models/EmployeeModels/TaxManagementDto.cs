using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class TaxManagementDto
    {
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal SocialSecurityTax { get; set; }
        public decimal MedicareTax { get; set; }
        public DateTime TaxYear { get; set; }
    }
}
