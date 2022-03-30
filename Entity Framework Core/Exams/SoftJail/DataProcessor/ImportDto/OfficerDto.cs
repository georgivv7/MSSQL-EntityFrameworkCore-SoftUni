namespace SoftJail.DataProcessor.ImportDto
{
    using SoftJail.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Xml.Serialization;

    [XmlType("Officer")]
    public class OfficerDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        [XmlElement("Name")]
        public string FullName{ get; set; }     

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        [XmlElement("Money")]
        public decimal Salary { get; set; }  
        [Required]
        [XmlElement("Position")]
        public string Position { get; set; }
        [Required]
        [XmlElement("Weapon")]
        public string Weapon { get; set; }

        [XmlElement("DepartmentId")]
        public int DepartmentId { get; set; }
        [XmlArray("Prisoners")]
        public OfficerPrisonersDto[] OfficerPrisoners { get; set; } 
    }
}
