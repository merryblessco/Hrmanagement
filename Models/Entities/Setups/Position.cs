using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.Setups
{
    public class Position : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
