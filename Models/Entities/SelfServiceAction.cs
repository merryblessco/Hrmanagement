using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class SelfServiceAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ActionID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public string ActionType { get; set; }// -- (e.g., leave request, benefit update)
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }//-- (e.g., pending, approved, rejected)
    }
}
