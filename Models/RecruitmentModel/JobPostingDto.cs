using HRbackend.Models.Entities.Setups;
using HRbackend.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.RecruitmentModel
{
    public class JobPostingDto
    {
        public Guid Id { get; set; }
        public string JobTitle { get; set; }
        public Guid DepartmentId { get; set; }
        public string Description { get; set; }
        public DateTime PostingDate { get; set; }
        public string Status { get; set; } // (e.g., open, closed)
        public JobMode JobMode { get; set; }
        public WorkMode WorkMode { get; set; }
        public string CompanyAddress { get; set; }
        public decimal MinSalaryRange { get; set; }
        public decimal MaxSalaryRange { get; set; }
        public List<string> Benefits { get; set; }
        public List<string> Responsibilities { get; set; }
        public List<string> Qualifications { get; set; }
    }

    public class JobPostingResponseDto : JobPostingDto
    {
        public string DepartmentName { get; set; }
        public string JobModeName { get; set; }
        public string WorkModeName { get; set; }
    }
}
