using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace HRbackend.Models.Entities.Employees
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string PassportPath { get; set; }
        public string ResumePath { get; set; }
        public string State { get; set; }
        public string LGA { get; set; }
        public string Password { get; set; }
        public string LoginId { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime DOB { get; set; }
        [ForeignKey("ManagerID")]
        public int ManagerID { get; set; }
        public bool IsAdmin { get; set; }
    }
}
