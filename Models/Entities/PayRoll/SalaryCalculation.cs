using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.PayRoll
{
    public class SalaryCalculation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SalaryCalculationId { get; set; }
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public string Fullname { get; set; }
        public string AdjustmentType { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deductions { get; set; }
        public string Resoans { get; set; }
        public DateTime CalculationDate { get; set; }
    }
}
