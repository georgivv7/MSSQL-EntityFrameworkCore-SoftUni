namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        private static int TicketsAvailable = 20;
        private static string MainCharacterFormat = "Plays main character in '{0}'.";
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatres = context.Theatres
                .ToArray()
                .Where(t => t.NumberOfHalls >= numbersOfHalls && t.Tickets.Count >= TicketsAvailable)
                .Select(t => new ExportTheatreDto
                {
                    Name = t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets
                        .ToArray()
                        .Where(tt => tt.RowNumber >= 1 && tt.RowNumber <= 5)
                        .Sum(tt => tt.Price),
                    Tickets = t.Tickets
                        .ToArray()
                        .Where(tt => tt.RowNumber >= 1 && tt.RowNumber <= 5)
                        .Select(tt => new ExportTicketDto
                        {
                            Price = tt.Price,
                            RowNumber = tt.RowNumber
                        })
                        .OrderByDescending(tt => tt.Price)
                        .ToArray()
                })
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(theatres, Formatting.Indented);
            return jsonString;
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            var sb = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ExportPlayDto[]), new XmlRootAttribute("Plays"));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter stringWriter = new StringWriter(sb);

            var plays = context.Plays
                .ToArray()
                .Where(p => p.Rating <= rating)
                .Select(p => new ExportPlayDto
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c", CultureInfo.InvariantCulture),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts
                        .ToArray()
                        .Where(c=>c.IsMainCharacter == true)
                        .Select(c => new ExportCastDto
                        {
                            FullName = c.FullName,
                            MainCharacter = string.Format(MainCharacterFormat,c.Play.Title)
                        })
                        .OrderByDescending(c=>c.FullName)
                        .ToArray()
                })
                .OrderBy(p=>p.Title)
                .ThenByDescending(p=>p.Genre)
                .ToArray();

            xmlSerializer.Serialize(stringWriter, plays, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
