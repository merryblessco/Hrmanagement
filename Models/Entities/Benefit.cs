using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class Benefit : BaseEntity
    {
 
        public string BenefitType { get; set; }// -- (e.g., health, dental, vision)
        public string Description { get; set; }
        public string Provider { get; set; }
}
}
