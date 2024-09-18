using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class OvertimeTracking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OvertimeID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public decimal OvertimeHours { get; set; }
        public string ApprovalStatus { get; set; } //(e.g., pending, approved)
    }
}
