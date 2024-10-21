using System.ComponentModel;

namespace HRbackend.Models.Enums
{
    public enum EmployeeCreationMode
    {
        [Description("Single")]
        Single = 1,
        [Description("Bulk")]
        Bulk
    }
}
