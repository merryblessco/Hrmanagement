using System.ComponentModel;

namespace HRbackend.Models.Enums
{
    public enum ApplicationRoles
    {
        Administrator,   // Full system access, including managing users and permissions
        HrManager,       // Can manage employee records, handle recruitment, and oversee HR functions
        Recruiter,       // Manages job postings, reviews applicants, and handles hiring
        Employee,        // Standard employee role with access to personal data and leave requests
        PayrollManager,  // Manages payroll data, salary information, and tax documents
        Manager,         // Can oversee specific teams, approve leaves, and view team performance
        ItSupport,       // Handles technical support, user access, and system maintenance
        Auditor,         // Has read-only access to audit data for compliance
        Trainer,         // Manages training materials, sessions, and employee learning progress
        Guest            // Limited, temporary access for external consultants or auditors
    }
}
