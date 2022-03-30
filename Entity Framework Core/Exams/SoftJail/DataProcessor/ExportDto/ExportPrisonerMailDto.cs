namespace SoftJail.DataProcessor.ExportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Message")]
    public class ExportPrisonerMailDto
    {
        [XmlElement("Description")]
        public string ReversedDescription { get; set; }
    }
}
