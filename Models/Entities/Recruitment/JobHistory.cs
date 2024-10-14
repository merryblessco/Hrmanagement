using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities.Recruitment
{
    public class JobHistory : BaseEntity
    {
        public Guid EmployeeID { get; set; }
        public string PreviousJobTitle { get; set; }
        public string PreviousDepartment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Employee Employee { get; set; }

    }
}
