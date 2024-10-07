using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.ApplicantsModel
{
    public class InterviewDto
    {
        public int JobID { get; set; }
        [ForeignKey("ApplicantID")]

        public int ApplicantID { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicatMobile { get; set; }
        public string MeetingLink { get; set; }
        public string MeetingNote { get; set; }
        public string Fullname { get; set; }
        public string? Feedback { get; set; }
        public List<string> Interviewers { get; set; }
        public DateTime InterviewDate { get; set; }
        public string Status { get; set; } //(e.g., scheduled, completed)
        public DateTime DateCreated { get; set; }
    }
}
