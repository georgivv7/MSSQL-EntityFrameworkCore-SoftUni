namespace Artillery.Data.Models
{
    using Artillery.Data.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Gun
    {
        public Gun()
        {
            CountriesGuns = new HashSet<CountryGun>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Manufacturer")]
        public int ManufacturerId { get; set; }
        [Required]
        public Manufacturer Manufacturer { get; set; }

        [Required]
        [Range(100,1350000)]
        public int GunWeight { get; set; }

        [Required]
        [Range(2.00,35.00)]
        public double BarrelLength { get; set; }

        public int? NumberBuild { get; set; }
        
        [Required]
        [Range(1,100000)]
        public int Range { get; set; }

        [Required]
        public GunType GunType { get; set; }

        [Required]
        [ForeignKey("Shell")]
        public int ShellId { get; set; }

        [Required]
        public Shell Shell { get; set; }

        public ICollection<CountryGun> CountriesGuns { get; set; }
    }
}