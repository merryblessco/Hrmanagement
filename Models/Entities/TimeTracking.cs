using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class TimeTracking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimeEntryID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public decimal HoursWorked { get; set; }
    }
}
