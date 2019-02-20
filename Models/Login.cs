using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class Logger
    {
        [Key]
        public int nothing {get; set;}

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address:")]
        public string lemail {get; set;}

        [Required]
        [MinLength(8)]
        [Display(Name = "Password")]
        public string lpassword {get; set;}

        public Logger ()
        {
        
        }
    }
}