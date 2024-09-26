using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.PayRoll
{
    public class PaySlip
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaySlipId { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public DateTime PayPeriod { get; set; }
        public decimal TotalEarnings { get; set; }
        public decimal TotalDeductions { get; set; }
        public DateTime DateGenerated { get; set; }
    }
}
