using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class Interview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterviewID { get; set; }
        [ForeignKey("ApplicantID")]
        public int ApplicantID { get; set; }
        public DateTime InterviewDate { get; set; }
        public string Feedback { get; set; }
        public string Interviewer { get; set; }
        public string Status { get; set; } //(e.g., scheduled, completed)
   
    }
}
