using HRbackend.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.ApplicantsModel
{
    public class InterviewDto
    {
        public Guid JobID { get; set; }

        public Guid ApplicantID { get; set; }
        public string ApplicantEmail { get; set; }
        public string ApplicatMobile { get; set; }
        public string MeetingLink { get; set; }
        public string MeetingNote { get; set; }
        public string Fullname { get; set; }
        public string? Feedback { get; set; }
        public List<string> Interviewers { get; set; }
        public DateTime InterviewDate { get; set; }
        public InterViewStatus Status { get; set; } //(e.g., scheduled, completed)
        public string StatusName { get; set; }

        public DateTime DateCreated { get; set; }
    }

    public class SendInvitationRequest
    {
        public Guid JobID { get; set; }
        public Guid ApplicantID { get; set; }
        public string MeetingLink { get; set; }
        public string MeetingNote { get; set; }
        public List<string> Interviewers { get; set; }
        public DateTime InterviewDate { get; set; }
    }

    public class InterviewRequest { 
        public Guid JobID { get; set; }

        public Guid ApplicantID { get; set; }
     
    }
}
