using System.ComponentModel;
using System.Reflection;

namespace HRbackend.Models.Enums
{
    public enum ApplicationStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Completed")]
        Completed = 2,
        [Description("Rejected")]
        Rejected = 3
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
