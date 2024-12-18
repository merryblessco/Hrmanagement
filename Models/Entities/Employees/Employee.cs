﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;
using HRbackend.Models.Enums;
using HRbackend.Models.Auth;
using HRbackend.Models.Entities.Setups;

namespace HRbackend.Models.Entities.Employees
{
    public class Employee : BaseEntity
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public ApplicationRoles? Role { get; set; }
        public EmployeeCreationMode? CreationMode { get; set; }
        public string? PhoneNumber { get; set; } = String.Empty;
        public string? AlternativePhoneNumber { get; set; } = String.Empty;
        public string? Address { get; set; } = String.Empty;
        public Guid? StateId { get; set; }
        public Guid? LGAId { get; set; }
        public DateTime? DOB { get; set; }

        public byte[]? PassportBytes { get; set; }
        public byte[]? ResumeBytes { get; set; }
        public State? State { get; set; }
        public LGA? Lga { get; set; }
        public string? SpouseInfo { get; set; } = String.Empty;
        public string? SpousePhoneNumber { get; set; } = String.Empty; 


    }
}
