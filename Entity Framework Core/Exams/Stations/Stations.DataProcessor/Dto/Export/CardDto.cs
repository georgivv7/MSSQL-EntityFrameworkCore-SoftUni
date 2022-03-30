namespace Stations.DataProcessor.Dto.Export
{
    using System.Xml.Serialization;

    [XmlType("Card")]
    public class CardDto
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("type")]
        public string Type { get; set; }
        public Export.TicketDto[] Tickets { get; set; } 
    }
}
