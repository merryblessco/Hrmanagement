using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Enums;

namespace HRbackend.Models.Entities.Recruitment
{
    public class Interview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterviewID { get; set; }

        public int ApplicantID { get; set; }
        public int JobID { get; set; }

        public string ApplicantEmail { get; set; }
        public string ApplicatMobile { get; set; }
        public string MeetingLink { get; set; }
        public string MeetingNote { get; set; }
        public string Fullname { get; set; }
        public string? Feedback { get; set; }
        public List<string> Interviewers { get; set; }
        public DateTime InterviewDate { get; set; }
        public InterViewStatus Status { get; set; } //(e.g., scheduled, completed)
        public DateTime DateCreated { get; set; }

        public Applicants Applicant { get; set; }
        


    }
}
