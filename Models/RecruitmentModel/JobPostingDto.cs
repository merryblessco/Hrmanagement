using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.RecruitmentModel
{
    public class JobPostingDto
    {
        public int JobID { get; set; }
        public string JobTitle { get; set; }
        public string JobCode { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public DateTime PostingDate { get; set; }
        public string Status { get; set; } // (e.g., open, closed)
        public string JobMode { get; set; }
        public string WorkMode { get; set; }
        public string CompanyAddress { get; set; }
        public decimal MinSalaryRange { get; set; }
        public decimal MaxSalaryRange { get; set; }
        public List<string> Benefits { get; set; }
        public List<string> Responsibilities { get; set; }
        public List<string> Qualifications { get; set; }
    }
}
