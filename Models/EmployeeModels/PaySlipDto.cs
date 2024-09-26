using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class PaySlipDto
    {
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public DateTime PayPeriod { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public DateTime DateGenerated { get; set; }
    }
}

