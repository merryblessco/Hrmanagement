using System.ComponentModel;
using System.Reflection;

namespace HRbackend.Models.Enums
{
    public enum ApplicationStatus
    {
        [Description("Applied")]
        Applied = 1,
        [Description("Screening")]
        Screening = 2,
        [Description("Interview")]
        Interview = 3,
        [Description("Offered")]
        Offered = 4,
        [Description("Hired")]
        Hired = 5,
        [Description("Rejected")]
        Rejected = 6
    }
    public enum OfferLetterStatus
    {
        [Description("Sent")]
        Sent = 1,
        [Description("Accepted")]
        Accepted
    }
    public enum WelcomeEmailStatus
    {
        [Description("Sent")]
        Sent = 1,
        [Description("Pending")]
        Pending
    }
    public enum PaperworkStatus
    {
        [Description("Completed")]
        Completed = 1,
        [Description("Pending")]
        Pending
    }
    public enum EquipmentStatus
    {
        [Description("Prepared")]
        Prepared = 1,
        [Description("In-progress")]
        Inprogress
    }
    public enum BenefitType
    {
        [Description("Health Insurance")]
        HealthInsurance = 1,
        [Description("Retirement-Plan")]
        RetirementPlan
    }
    
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                {
                    return attribute.Description;
                }
            }
            return value.ToString(); // Fallback to the enum name if no description is found
        }
    }
}
