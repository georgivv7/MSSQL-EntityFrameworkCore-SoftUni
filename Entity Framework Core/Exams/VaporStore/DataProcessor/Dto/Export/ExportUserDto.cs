﻿namespace VaporStore.DataProcessor.Dto.Export
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("User")]
    public class ExportUserDto
    {
        [XmlAttribute("username")]
        public string Username { get; set; }
            
        [XmlArray("Purchases")]
        public ExportUserPurchaseDto[] Purchases { get; set; }
        
        [XmlElement("TotalSpent")]
        public decimal TotalSpent { get; set; }

    }
}
