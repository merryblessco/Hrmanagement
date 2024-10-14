using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities
{
    public class TimeTracking : BaseEntity
    {
        public Guid EmployeeID { get; set; }

        public DateTime Date { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime TimeOut { get; set; }
        public decimal HoursWorked { get; set; }
        public Employee Employee { get; set; }

    }
}
