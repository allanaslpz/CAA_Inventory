
using caa_mis.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace caa_mis.Models
{
    public class Employee
    {
        public int ID { get; set; }


        //[Required]
        //public int UserAccountID { get; set; }
        //public UserAccountID UserAccountID { get; set; }
        public string FullName
        {
            get
            {
                return FirstName
                    +  " "
                    + LastName;
            }
        }
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }        

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Branch")]
        public BranchRoles BranchRoles { get; set; } = BranchRoles.None;//Not needed

        //public UserVM UserVM { get; set; }

        public Archived Status { get; set; }

        public ICollection<Bulk> Bulks { get; set; } = new HashSet<Bulk>();
        public ICollection<Event> Events { get; set; } = new HashSet<Event>();
        public ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
