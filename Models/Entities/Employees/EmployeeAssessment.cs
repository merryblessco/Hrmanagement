using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.Employees
{
    public class EmployeeAssessment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssessmentID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public DateTime AssessmentDate { get; set; }
        public string SelfEvaluation { get; set; }
        public string Goals { get; set; }
    }
}
