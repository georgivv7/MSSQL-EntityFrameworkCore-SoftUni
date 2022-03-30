namespace Artillery.DataProcessor.ImportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Country")]
    public class ImportCountryDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(60)]
        [XmlElement("CountryName")]
        public string CountryName { get; set; }

        [Required]
        [XmlElement("ArmySize")]
        [Range(50000,10000000)]
        public int ArmySize { get; set; }   
    }
}
