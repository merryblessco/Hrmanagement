using HRbackend.Models.Entities.Setups;

namespace HRbackend.Models.Entities.Employees
{
    public class EmployeeContract : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public string? JobTitle { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid PositionId { get; set; }
        public DateTime HireDate { get; set; }
        public Guid? ManagerId { get; set; }
        public Employee Manager { get; set; }
        public Department Department { get; set; }
        public Position Position { get; set; }
        public Employee Employee { get; set; }

    }
}
