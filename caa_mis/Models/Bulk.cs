using System.ComponentModel.DataAnnotations;

namespace caa_mis.Models
{
    public class Bulk
    {
        public int ID { get; set; }

        [Required]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public int TransactionStatusID { get; set; }
        public TransactionStatus TransactionStatus { get; set; }

        [Required]
        public int BranchID { get; set; }
        public Branch Branch { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public ICollection<BulkItem> BulkItems { get; set; } = new HashSet<BulkItem>();

    }
}
