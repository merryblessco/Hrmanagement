using HRbackend.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Payroll
{
    public class TaxDto
    {
        public string TaxName { get; set; }

        [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100")]
        public decimal? TaxPercentage { get; set; }
    }
}
