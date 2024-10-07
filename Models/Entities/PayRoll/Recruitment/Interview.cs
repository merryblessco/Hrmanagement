﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.Recruitment
{
    public class Interview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InterviewID { get; set; }

        [ForeignKey("ApplicantID")]

        public int ApplicantID { get; set; }
        public int JobID { get; set; }

        [ForeignKey("ApplicantEmail")]
        public string ApplicantEmail { get; set; }
        public string ApplicatMobile { get; set; }
        public string MeetingLink { get; set; }
        public string MeetingNote { get; set; }
        public string Fullname { get; set; }
        public string? Feedback { get; set; }
        public List<string> Interviewers { get; set; }
        public DateTime InterviewDate { get; set; }
        public int Status { get; set; } //(e.g., scheduled, completed)
        public DateTime DateCreated { get; set; }


    }
}
