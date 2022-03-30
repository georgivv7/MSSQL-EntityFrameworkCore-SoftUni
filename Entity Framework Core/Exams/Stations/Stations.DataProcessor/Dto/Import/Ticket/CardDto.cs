namespace Stations.DataProcessor.Dto.Import.Ticket
{
	using System.ComponentModel.DataAnnotations;
	using System.Xml.Serialization;

	[XmlType("Card")]
	public class CardDto
	{
		[Required]
		[XmlAttribute("Name")]
		public string Name { get; set; }
	}
}
