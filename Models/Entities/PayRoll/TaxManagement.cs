using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities.PayRoll
{
    public class TaxManagement : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal SocialSecurityTax { get; set; }
        public decimal MedicareTax { get; set; }
        public DateTime TaxYear { get; set; }
        public Employee Employee { get; set; }

    }
}
