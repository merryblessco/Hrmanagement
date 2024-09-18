using HRbackend.Models.Entities;
using LinkOrgNet.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRbackend.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Applicants> Applicants { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<EmployeeAssessment> EmployeeAssessments { get; set; }
        public DbSet<EmployeeBenefit> EmployeeBenefits { get; set; }
        public DbSet<EmployeeDocuments> EmployeeDocuments { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<JobHistory> JobHistorys { get; set; }
        public DbSet<JobPostings> JobPostings { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Onboarding> Onboardings { get; set; }
        public DbSet<OvertimeTracking> OvertimeTrackings { get; set; }
        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<SelfServiceAction> SelfServiceAction { get; set; }
        public DbSet<TimeTracking> TimeTracking { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<LGA> LGAs { get; set; }
    }
}
