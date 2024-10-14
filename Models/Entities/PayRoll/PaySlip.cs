using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities.PayRoll
{
    public class PaySlip : BaseEntity
    {
        public Guid EmployeeID { get; set; }
        public DateTime PayPeriod { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public DateTime DateGenerated { get; set; }
        public Employee Employee { get; set; }
    }
}
