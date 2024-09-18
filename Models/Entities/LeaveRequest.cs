using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class LeaveRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaveRequestID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public string LeaveType  {get; set; }// (e.g., vacation, sick leave)
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // (e.g., pending, approved, rejected)
    }
}
