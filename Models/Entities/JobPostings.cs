﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HRbackend.Models.Entities
{
    public class JobPostings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int JobID { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public string Description { get; set; }
        public DateTime PostingDate { get; set; }
        public string Status { get; set; } // (e.g., open, closed)
}
}
