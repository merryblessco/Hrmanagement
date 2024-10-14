using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HRbackend.Models.Entities.Setups;
using HRbackend.Models.Enums;

namespace HRbackend.Models.Entities.Recruitment
{
    public class JobPostings : BaseEntity
    {
        public string JobTitle { get; set; }
        public string JobCode { get; set; }
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

        public Department Department { get; set; }

    }
}
