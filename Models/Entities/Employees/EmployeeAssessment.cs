using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.Employees
{
    public class EmployeeAssessment : BaseEntity
    {
        public Guid EmployeeID { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string SelfEvaluation { get; set; }
        public string Goals { get; set; }
        public Employee Employee { get; set; }

    }
}
