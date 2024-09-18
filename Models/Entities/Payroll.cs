using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class Payroll
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PayrollID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal Deductions { get; set; }
        public decimal Tax { get; set; }
        public DateTime PayDate { get; set; }
        public string PayStubFilePath { get; set; }
    }
}
