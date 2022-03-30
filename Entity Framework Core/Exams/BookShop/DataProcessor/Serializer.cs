namespace BookShop.DataProcessor
{
    using System;
    using Data.Models.Enums;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors
                .Select(a => new 
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks
                        .Select(ab=>ab.Book)
                        .OrderByDescending(b=>b.Price)
                        .Select(b => new
                        {
                            BookName = b.Name,
                            BookPrice = b.Price.ToString("F2")
                        })
                        .ToArray()
                })
                .ToArray()
                .OrderByDescending(a => a.Books.Length)
                .ThenBy(a => a.AuthorName)
                .ToArray();

            var jsonString = JsonConvert.SerializeObject(authors, Formatting.Indented);
            return jsonString;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books
                .Where(b => b.PublishedOn < date && b.Genre == Genre.Science)
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b => b.PublishedOn)
                .Select(b => new ExportBookDto
                {
                    Pages = b.Pages,
                    Name = b.Name,  
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture)
                })
                .Take(10)
                .ToArray();

            var sb = new StringBuilder();
            var xmlSerializer = new XmlSerializer(typeof(ExportBookDto[]), new XmlRootAttribute("Books"));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName() });
            xmlSerializer.Serialize(new StringWriter(sb), books, namespaces);

            string result = sb.ToString().TrimEnd();
            return result;
        }
    }
}