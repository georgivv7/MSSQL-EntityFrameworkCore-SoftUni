namespace BookShop.DataProcessor.ExportDto
{
    using System;
    using System.Xml.Serialization;

    [XmlType("Book")]
    public class ExportBookDto
    {
        [XmlAttribute("Pages")]
        public int Pages { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
