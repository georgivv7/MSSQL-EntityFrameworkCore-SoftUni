namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {

            var prisoners = context.Prisoners
                    .ToArray()
                    .Where(p => ids.Contains(p.Id))
                    .Select(p => new ExportPrisonerDto
                    {
                        Id = p.Id,
                        Name = p.FullName,
                        CellNumber = p.Cell.CellNumber,
                        Officers = p.PrisonerOfficers
                            .Select(po => new ExportOfficerDto
                            {
                                OfficerName = po.Officer.FullName,
                                Department = po.Officer.Department.Name
                            })
                            .OrderBy(o => o.OfficerName)
                            .ToArray(),
                        TotalOfficerSalary = Math.Round(p.PrisonerOfficers.Sum(po => po.Officer.Salary), 2)
                    })
                    .OrderBy(p=>p.Name)
                    .ThenBy(p=>p.Id)
                    .ToArray();

            var jsonString = JsonConvert.SerializeObject(prisoners,Formatting.Indented);
            return jsonString;
        }
        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var sb = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(XmlExportPrisonerDto[]), new XmlRootAttribute("Prisoners"));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var prisonerNames = prisonersNames.Split(",").ToArray();

            var prisoners = context.Prisoners
                .ToArray()
                .Where(p => prisonerNames.Contains(p.FullName))
                .Select(p => new XmlExportPrisonerDto
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Mails = p.Mails
                        .Select(m=> new ExportPrisonerMailDto 
                        {
                            ReversedDescription = String.Join("",m.Description.Reverse())
                        })
                        .ToArray()
                })
                .OrderBy(p=>p.Name)
                .ThenBy(p=>p.Id)
                .ToArray();

            using (StringWriter stringReader = new StringWriter(sb))
            {
                xmlSerializer.Serialize(stringReader, prisoners, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

    }   
}