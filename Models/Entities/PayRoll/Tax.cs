using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities.PayRoll
{
    public class Tax : BaseEntity
    {
        public string TaxName { get; set; }
        public decimal? TaxPercentage { get; set; }
    }
}
