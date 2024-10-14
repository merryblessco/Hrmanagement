using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace HRbackend.Models.Entities.Employees
{
    public class EmployeeDocuments : BaseEntity
    {
      
        public Guid DocumentID { get; set; }
        public Guid EmployeeID { get; set; }
        public string DocumentType { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
