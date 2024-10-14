using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRbackend.Models.Entities.Setups
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}
