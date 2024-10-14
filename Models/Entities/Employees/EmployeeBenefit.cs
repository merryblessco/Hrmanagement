namespace HRbackend.Models.Entities.Employees
{
    public class EmployeeBenefit : BaseEntity
    {
        public Guid EmployeeID { get; set; }
        public Guid BenefitID { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; }// -- (e.g., active, terminated)
        public Employee Employee { get; set; }

    }
}
