using HRbackend.Models.Entities.PayRoll;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class EmployeeDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public IFormFile Passport { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public string Password { get; set; }
        public string LoginId { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime DOB { get; set; }
        [ForeignKey("ManagerID")]
        public int ManagerID { get; set; }
        public bool IsAdmin { get; set; }
        public byte[] PassporthFile { get; set; }
        public ICollection<SalaryCalculation> SalaryCalculations { get; set; }
        public ICollection<PaySlip> PaySlips { get; set; }
        public ICollection<TaxManagement> Taxes { get; set; }
        public ICollection<BenefitAdministration> BenefitsAdmin { get; set; }
    }
}
