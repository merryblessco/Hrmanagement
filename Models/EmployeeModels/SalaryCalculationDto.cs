using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class SalaryCalculationDto
    {
        public Guid EmployeeId { get; set; }
        public string Fullname { get; set; }
        public string AdjustmentType { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal Deductions { get; set; }
        public string Resoans { get; set; }
        public DateTime CalculationDate { get; set; }
    }
}
