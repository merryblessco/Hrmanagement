using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Employees;

namespace HRbackend.Models.Entities
{
    public class PerformanceReview : BaseEntity
    {
        public Guid EmployeeID { get; set; }

        public DateTime ReviewDate { get; set; }
        public string Goals { get; set; }
        public string Feedback { get; set; }
        public string Reviewer { get; set; }
        public int Rating { get; set; }
        public Employee Employee { get; set; }

    }
}
