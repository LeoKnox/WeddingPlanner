
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key]
        public int WeddingId {get; set;}

        [Required]
        [MinLength(2)]
        [Display(Name = "Wedder One")]
        public string WedderOne {get; set;}

        [Required]
        [MinLength(2)]
        [Display(Name = "Wedder Two")]
        public string WedderTwo {get; set;}

        [Required]
        [Display(Name = "Date")]
        public DateTime Date {get; set;}

        [Required]
        [Display(Name = "Wedding Address")]
        public String Address {get; set;}

        public List<Guest> Attendee {get; set;}
    }
}