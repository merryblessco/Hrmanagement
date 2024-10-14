using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities
{
    public class OvertimeTracking : BaseEntity
    {
        public Guid EmployeeID { get; set; }

        public decimal OvertimeHours { get; set; }
        public string ApprovalStatus { get; set; } //(e.g., pending, approved)
        public Employee Employee { get; set; }

    }
}
