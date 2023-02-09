using System.ComponentModel.DataAnnotations;
namespace caa_mis.Models
{
    public class TransactionItem
    {
        public int ID { get; set; }

        [Required]
        public int ItemID { get; set; }
        public Item Item { get; set; }

        [Required]
        public int TransactionID { get; set; }
        public Transaction Transaction { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
