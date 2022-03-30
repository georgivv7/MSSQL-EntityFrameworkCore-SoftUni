namespace VaporStore.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    public class User
    {
        public User()
        {
            Cards = new HashSet<Card>();
        }
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{1}[a-z]+ [A-Z]{1}[a-z]+$")]  
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Range(3,103)]
        public int Age { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
