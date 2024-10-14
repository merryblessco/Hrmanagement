using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities
{
    public class SelfServiceAction : BaseEntity
    {
        public Guid EmployeeID { get; set; }

        public string ActionType { get; set; }// -- (e.g., leave request, benefit update)
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }//-- (e.g., pending, approved, rejected)
        public Employee Employee { get; set; }

    }
}
