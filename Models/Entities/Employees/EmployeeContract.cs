using HRbackend.Models.Entities;
using HRbackend.Models.Entities.Employees;
using HRbackend.Models.Entities.Setups;
using HRbackend.Models.Enums;

public class EmployeeContract : BaseEntity
{
    public Guid EmployeeId { get; set; }

    // Unique Employee Number
    public string? EmployeeNumber { get; private set; }
    public EmployeeStatus? Status { get; set; }

    public string? JobTitle { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? PositionId { get; set; }
    public DateTime? HireDate { get; set; }
    public Guid? ManagerId { get; set; }
    public Employee Manager { get; set; }
    public Department Department { get; set; }
    public Position Position { get; set; }
    public Employee? Employee { get; set; }
    public bool? IsOnboardingComplete{ get; set; } = false;
    public DateTime? DateJoined { get; set; }

    public EmployeeContract()
    {
        // Generate a unique Employee Number during construction
        EmployeeNumber = GenerateUniqueEmployeeNumber();
    }

    private string GenerateUniqueEmployeeNumber()
    {
        // Generate a new GUID
        Guid guid = Guid.NewGuid();

        // Take a substring to create a unique number
        string uniquePart = guid.ToString("N").Substring(0, 4).ToUpper(); // Example: EMP-1234

        // Combine with a prefix
        return $"EMP-{uniquePart}";
    }
}
