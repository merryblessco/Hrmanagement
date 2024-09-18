using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class PerformanceReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Goals { get; set; }
        public string Feedback { get; set; }
        public string Reviewer { get; set; }
        public int Rating { get; set; }
    }
}
