using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace HRbackend.Models.Entities.Employees
{
    public class EmployeeDocuments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentID { get; set; }
        [ForeignKey("EmployeeID")]
        public int EmployeeID { get; set; }
        public string DocumentType { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
