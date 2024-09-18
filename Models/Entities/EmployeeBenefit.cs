using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class EmployeeBenefit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeBenefitID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        [ForeignKey("BenefitID")]
        public int BenefitID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; }// -- (e.g., active, terminated)
    }
}
