using HRbackend.Models.Auth;
using HRbackend.Models.Entities;
using HRbackend.Models.Entities.Employees;
using HRbackend.Models.Entities.PayRoll;
using HRbackend.Models.Entities.Recruitment;
using HRbackend.Models.Entities.Setups;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HRbackend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        // ARRANGE ALPHABETICALLY
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Applicants> Applicants { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<BenefitAdministration> BenefitAdmin { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeContract> EmployeeContracts { get; set; }
        public DbSet<EmployeeAssessment> EmployeeAssessments { get; set; }
        public DbSet<EmployeeBenefit> EmployeeBenefits { get; set; }
        public DbSet<EmployeeDocuments> EmployeeDocuments { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<JobHistory> JobHistorys { get; set; }
        public DbSet<JobPostings> JobPostings { get; set; }
        public DbSet<LGA> LGAs { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Onboarding> Onboardings { get; set; }
        public DbSet<OvertimeTracking> OvertimeTrackings { get; set; }
        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<PaySlip> PaySlips { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<SalaryCalculation> SalaryCalculations { get; set; }
        public DbSet<SelfServiceAction> SelfServiceAction { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Tax> TaxInfo { get; set; }
        public DbSet<TaxManagement> Taxes { get; set; }
        public DbSet<TimeTracking> TimeTracking { get; set; }
    }
}
