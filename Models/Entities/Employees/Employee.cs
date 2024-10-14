using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;
using HRbackend.Models.Enums;
using HRbackend.Models.Auth;
using HRbackend.Models.Entities.Setups;

namespace HRbackend.Models.Entities.Employees
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public ApplicationRoles Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Guid StateId { get; set; }
        public Guid LGAId { get; set; }
        public DateTime DOB { get; set; }
        public byte[] PassportBytes { get; set; }
        public byte[] ResumeBytes { get; set; }
        public State State { get; set; }
        public LGA Lga { get; set; }

    }
}
