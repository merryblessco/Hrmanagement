using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities
{
    public class LeaveRequest : BaseEntity
    {
        public Guid EmployeeID { get; set; }

        public string LeaveType  {get; set; }// (e.g., vacation, sick leave)
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // (e.g., pending, approved, rejected)
        public Employee Employee { get; set; }

    }
}
