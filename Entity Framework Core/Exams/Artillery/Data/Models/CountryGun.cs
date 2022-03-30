namespace Artillery.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    public class CountryGun
    {
        [Required]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Required]
        public int GunId { get; set; }
        public Gun Gun { get; set; }
    }
}