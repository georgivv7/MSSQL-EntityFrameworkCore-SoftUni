﻿namespace SoftJail.DataProcessor.ExportDto
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Prisoner")]
    public class XmlExportPrisonerDto
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("IncarcerationDate")]
        public string IncarcerationDate { get; set; }  
        [XmlArray("EncryptedMessages")]
        public ExportPrisonerMailDto[] Mails { get; set; }  
    }
}
