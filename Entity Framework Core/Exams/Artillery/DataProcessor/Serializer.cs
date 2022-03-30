namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.DataProcessor.ExportDto;
    using Data.Models.Enums;
    using System.Linq;
    using Newtonsoft.Json;
    using System.Text;
    using System.Xml.Serialization;
    using System.IO;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {
            var shells = context.Shells
                .ToArray()
                .Where(s => s.ShellWeight > shellWeight)
                .Select(s => new ExportShellDto
                {
                    ShellWeight = s.ShellWeight,
                    Caliber = s.Caliber,
                    Guns = s.Guns
                        .ToArray()
                        .Where(g => g.GunType == GunType.AntiAircraftGun)                     
                        .Select(g => new ExportGunDto
                        {
                            GunType = g.GunType.ToString(),
                            GunWeight = g.GunWeight,
                            BarrelLength = g.BarrelLength,
                            Range = g.Range > 3000 ? "Long-range" : "Regular range"
                        })
                        .OrderByDescending(g => g.GunWeight)
                        .ToArray()
                })
                .OrderBy(s => s.ShellWeight)
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(shells, Formatting.Indented);
            return jsonString;
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {
            var sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);

            var xmlSerializer = new XmlSerializer(typeof(ExportGunsDto[]), new XmlRootAttribute("Guns"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var guns = context.Guns
                .ToArray()
                .Where(g => g.Manufacturer.ManufacturerName == manufacturer)
                .Select(g => new ExportGunsDto
                {
                    Manufacturer = g.Manufacturer.ManufacturerName,
                    GunType = g.GunType.ToString(),
                    GunWeight = g.GunWeight,
                    Barrellength = g.BarrelLength,
                    Range = g.Range,
                    Countries = g.CountriesGuns
                        .ToArray()
                        .Where(c => c.Country.ArmySize > 4500000)
                        .Select(c => new ExportCountryDto
                        {
                            Country = c.Country.CountryName,
                            ArmySize = c.Country.ArmySize
                        })
                        .OrderBy(c => c.ArmySize)
                        .ToArray()

                })
                .OrderBy(g => g.Barrellength)
                .ToArray();

            xmlSerializer.Serialize(stringWriter, guns, namespaces);

            string xmlString = sb.ToString().TrimEnd();
            return xmlString;
        }
    }
}
