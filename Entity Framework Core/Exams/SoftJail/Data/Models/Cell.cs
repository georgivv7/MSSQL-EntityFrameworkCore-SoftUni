namespace SoftJail.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Cell
    {
        public Cell()
        {
            Prisoners = new HashSet<Prisoner>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1,1000)]
        public int CellNumber { get; set; }
        [Required]
        public bool HasWindow { get; set; }

        [Required]
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        [Required]
        public Department Department { get; set; }

        public ICollection<Prisoner> Prisoners { get; set; }

    }
}
