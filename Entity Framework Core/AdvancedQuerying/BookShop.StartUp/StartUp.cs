namespace BookShop
{
    using BookShop.Data;
    using BookShop.Models.Enums;
    using BookShop.Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        static void Main()
        {
            var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //string input = Console.ReadLine();
            int result = RemoveBooks(db);
            Console.WriteLine(result);
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var books = context.Books
                    .AsEnumerable()
                    .Where(b => b.AgeRestriction
                        .ToString().ToLower() == command.ToLower())
                    .Select(b => b.Title)
                    .OrderBy(b => b)
                    .ToList();

            string result = String.Join(Environment.NewLine, books);
            return result;
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books
                    .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                    .OrderBy(b => b.BookId)
                    .Select(b => b.Title)
                    .ToList();

            return String.Join(Environment.NewLine, goldenBooks);
        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                    .Where(b => b.Price > 40)
                    .OrderByDescending(b => b.Price)
                    .Select(b => new
                    {
                        b.Title,
                        b.Price
                    })
                    .ToList();

            StringBuilder builder = new StringBuilder();

            foreach (var b in books)
            {
                builder.AppendLine($"{b.Title} - ${b.Price:F2}");
            }

            return builder.ToString().Trim();
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                    .Where(b => b.ReleaseDate.Value.Year != year)
                    .OrderBy(b => b.BookId)
                    .Select(b => b.Title)
                    .ToList();

            return String.Join(Environment.NewLine, books);
        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categoriesList = input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                                      .Select(c => c.ToLower())
                                      .ToList();

            var booksByCategory = new List<string>();

            foreach (var category in categoriesList)
            {
                var books = context.Books
                    .Where(b => b.BookCategories.Any(c => c.Category.Name.ToLower() == category))
                    .Select(b => b.Title)
                    .ToList();

                booksByCategory.AddRange(books);
            }

            return String.Join(Environment.NewLine, booksByCategory.OrderBy(c => c));
        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dateFormatted = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                    .Where(b => b.ReleaseDate < dateFormatted)
                    .OrderByDescending(b => b.ReleaseDate)
                    .Select(b => new
                    {
                        b.Title,
                        b.EditionType,
                        b.Price
                    })
                    .ToList();

            StringBuilder builder = new StringBuilder();

            foreach (var b in books)
            {
                builder.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:F2}");
            }

            return builder.ToString().Trim();
        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                    .Where(a => a.FirstName.EndsWith(input))
                    .Select(a => a.FirstName + " " + a.LastName)
                    .OrderBy(a => a)
                    .ToList();

            return String.Join(Environment.NewLine, authors);
        }
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                    .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                    .Select(b => b.Title)
                    .OrderBy(b => b)
                    .ToList();

            return String.Join(Environment.NewLine, books);
        }
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                    .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                    .OrderBy(b => b.BookId)
                    .Select(b => new
                    {
                        b.Title,
                        FullName = string.Concat(b.Author.FirstName," ", b.Author.LastName)
                    })
                    .ToList();

            StringBuilder builder = new StringBuilder();

            foreach (var b in books)
            {
                builder.AppendLine($"{b.Title} ({b.FullName})");
            }

            return builder.ToString().Trim();
        }
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                    .Where(b => b.Title.Count() > lengthCheck)
                    .Select(b => b.Title)
                    .ToList();

            return books.Count();
        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                    .Select(a => new
                    {
                        FullName = string.Concat(a.FirstName, " ", a.LastName),
                        TotalCopies = a.Books.Sum(b => b.Copies)
                    })
                    .OrderByDescending(a => a.TotalCopies)
                    .ToList();

            StringBuilder builder = new StringBuilder();
            foreach (var a in authors)
            {
                builder.AppendLine($"{a.FullName} - {a.TotalCopies}");
            }

            return builder.ToString().Trim();
        }
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                    .Select(c => new
                    {
                        c.Name,
                        TotalProfit = c.CategoryBooks
                        .Select(cb => new
                        {
                            BookProfit = cb.Book.Price * cb.Book.Copies
                        })
                        .Sum(cat => cat.BookProfit)
                    })
                    .OrderByDescending(c => c.TotalProfit)
                    .ThenBy(c => c.Name)
                    .ToList();

            StringBuilder builder = new StringBuilder();

            foreach (var c in categories)
            {
                builder.AppendLine($"{c.Name} ${c.TotalProfit:F2}");
            }

            return builder.ToString().Trim();
        }
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                    .Select(c => new
                    {
                        c.Name,
                        TopBooks = c.CategoryBooks
                        .OrderByDescending(b => b.Book.ReleaseDate)
                        .Take(3)
                        .Select(cb => new
                        {
                            cb.Book.Title,
                            Year = cb.Book.ReleaseDate.Value.Year
                        })                                              
                    })
                    .OrderBy(c => c.Name)
                    .ToList();
            StringBuilder builder = new StringBuilder();

            foreach (var c in categories)
            {
                builder.AppendLine($"--{c.Name}");

                foreach (var book in c.TopBooks)
                {
                    builder.AppendLine($"{book.Title} ({book.Year})");
                }
            }
            string result = builder.ToString().Trim();

            return result;
        }
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                    .Where(b => b.ReleaseDate.Value.Year < 2010)
                    .ToList();

            foreach (var book in books)
            {
                book.Price += 5m;
            }

            context.SaveChanges();
                    
        }
        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context.Books
                    .Where(b => b.Copies < 4200)
                    .ToList();

            int numberOfBooksDeleted = booksToRemove.Count();
            context.Books.RemoveRange(booksToRemove);
            context.SaveChanges();

            return numberOfBooksDeleted;
        }
    }
}
