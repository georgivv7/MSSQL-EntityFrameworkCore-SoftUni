namespace TeisterMask.DataProcessor.ExportDto
{
    using System.Xml.Serialization;
    using TeisterMask.Data.Models.Enums;

    [XmlType("Project")]
    public class ExportProjectDto
    {
        [XmlAttribute("TasksCount")]
        public int TasksCount { get; set; }
        
        [XmlElement("ProjectName")]
        public string Name { get; set; }    
        
        [XmlElement("HasEndDate")]
        public string HasEndDate { get; set; }  
        
        [XmlArray("Tasks")]
        public ExportXmlTaskDto[] Tasks { get; set; }   
    }
}
