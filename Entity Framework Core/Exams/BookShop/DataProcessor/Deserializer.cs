namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(BookDto[]), new XmlRootAttribute("Books"));
            var deserializedBookDtos = (BookDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            var validBooks = new List<Book>();

            var sb = new StringBuilder();

            foreach (var bookDto in deserializedBookDtos)
            {
                if (!IsValid(bookDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var date = DateTime.ParseExact(bookDto.PublishedOn, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                var book = new Book()
                {
                    Name = bookDto.Name,
                    Genre = (Genre)bookDto.Genre,
                    Price = bookDto.Price,
                    Pages = bookDto.Pages,
                    PublishedOn = date
                };
                validBooks.Add(book);
                sb.AppendLine(String.Format(SuccessfullyImportedBook, book.Name, book.Price));
            }
            context.Books.AddRange(validBooks);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var deserializedAuthors = JsonConvert.DeserializeObject<AuthorDto[]>(jsonString);

            var authors = new List<Author>();

            var sb = new StringBuilder();

            foreach (var authorDto in deserializedAuthors)
            {
                if (!IsValid(authorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool emailExists = authors.FirstOrDefault(a => a.Email == authorDto.Email) != null;
                if (emailExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var author = new Author()
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Phone = authorDto.Phone,
                    Email = authorDto.Email
                };

                //var uniqueBookIds = authorDto.Books.Distinct();
                foreach (var authorDtoAuthorBookDto in authorDto.Books)
                {
                    var book = context.Books.Find(authorDtoAuthorBookDto.Id);

                    if (book == null)
                    {
                        continue;
                    }

                    author.AuthorsBooks.Add(new AuthorBook()
                    {
                        Author = author,
                        Book = book
                    });
                }

                if (author.AuthorsBooks.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                authors.Add(author);
                string authorFullName = author.FirstName + " " + author.LastName;
                sb.AppendLine(String.Format(SuccessfullyImportedAuthor, authorFullName, author.AuthorsBooks.Count));
            }
            context.Authors.AddRange(authors);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}