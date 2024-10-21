using HRbackend.Models.Entities.PayRoll;
using HRbackend.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.EmployeeModels
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }
        [Obsolete]
        public string? EmployeeNumber { get; set; }

        public string Address { get; set; }
        public string StateCode { get; set; }
        public Guid LGAId { get; set; }
        [Obsolete]
        public string? State { get; set; }
        [Obsolete]
        public string? Lga { get; set; }
        public DateTime DOB { get; set; }
        [Obsolete]
        public byte[]? PassportBytes { get; set; }
        [Obsolete]
        public byte[]? ResumeBytes { get; set; }
        public IFormFile Passport { get; set; }
        public IFormFile Resume { get; set; }

        //Contract
        public string? JobTitle { get; set; }
        public Guid DepartmentId { get; set; }
        [Obsolete]
        public string? Department { get; set; }
        public Guid PositionId { get; set; }
        [Obsolete]
        public string? Position { get; set; }

        public DateTime? HireDate { get; set; }
        public Guid? ManagerId { get; set; }
        [Obsolete]
        public string? ManagerName { get; set; }
        public string? StatusName { get; set; }
        public EmployeeStatus? Status { get; set; }


        //[Obsolete]
        //public string? Role { get; set; }

    }

    public class EmployeeExcelDto
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternativePhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public string SpouseInfo { get; set; }
        public string SpousePhoneNumber { get; set; }
        public string DateJoined { get; set; }
    }

}
