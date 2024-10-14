using HRbackend.Models.Entities.PayRoll;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class EmployeeContractDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;   
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Obsolete]
        public string JobTitle { get; set; }
        public string Address { get; set; }
        public string PositionId { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public IFormFile Passport { get; set; }
        public IFormFile Resume { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public DateTime DOB { get; set; }
        public ICollection<SalaryCalculation> SalaryCalculations { get; set; }
        public ICollection<PaySlip> PaySlips { get; set; }
        public ICollection<TaxManagement> Taxes { get; set; }
        public ICollection<BenefitAdministration> BenefitsAdmin { get; set; }
    }
}
