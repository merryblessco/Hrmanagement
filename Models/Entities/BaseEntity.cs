using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            this.CreatedDate = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public string? CreatedByName { get; set; }

        public string? CreatedByIp { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedByIp { get; }

        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }

}
