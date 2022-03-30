namespace Stations.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Station
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Town { get; set; }

        public ICollection<Trip> TripsTo { get; set; }
        public ICollection<Trip> TripsFrom { get; set; }    
    }
}
