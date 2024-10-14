using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities
{
    public class Payroll : BaseEntity
    {
        public Guid EmployeeID { get; set; }

        public decimal GrossSalary { get; set; }
        public decimal NetSalary { get; set; }
        public decimal Deductions { get; set; }
        public decimal Tax { get; set; }
        public DateTime PayDate { get; set; }
        public string PayStubFilePath { get; set; }
        public Employee Employee { get; set; }

    }
}
