namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        private static string ErrorMessage = "Invalid Data";
        private static string SuccessfullyAddedGamesMessage = "Added {0} ({1}) with {2} tags";
        private static string SuccessfullyAddedUsersMessage = "Imported {0} with {1} cards";    
        private static string SuccessfullyAddedPurchasesMessage = "Imported {0} for {1}";    
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var gamesDtos = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            var games = new List<Game>();
            var developers = new List<Developer>();
            var genres = new List<Genre>();
            var tags = new List<Tag>();

            foreach (var gameDto in gamesDtos)
            {
                if (!IsValid(gameDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime releaseDate;
                bool isDateValid = DateTime.TryParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out releaseDate);
                if (!isDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (gameDto.Tags.Length == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var game = new Game
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = releaseDate
                };

                var developer = developers.FirstOrDefault(d => d.Name == gameDto.Developer);               

                if (developer == null)
                {
                    var newDev = new Developer()
                    {
                        Name = gameDto.Developer
                    };

                    developers.Add(newDev);
                    game.Developer = newDev;
                }
                else
                {
                    game.Developer = developer;
                }

                var genre = genres.FirstOrDefault(g => g.Name == gameDto.Genre);
                if (genre == null)
                {
                    var newGenre = new Genre()
                    {
                        Name = gameDto.Genre
                    };
                    
                    genres.Add(newGenre);
                    game.Genre = newGenre;
                }
                else
                {
                    game.Genre = genre;
                }                       

                foreach (var gameTag in gameDto.Tags)
                {
                    if (String.IsNullOrEmpty(gameTag))
                    {
                        continue;
                    }

                    var tag = tags.FirstOrDefault(t => t.Name == gameTag);

                    if (tag == null)
                    {
                        var newTag = new Tag()
                        {
                            Name = gameTag
                        };

                        tags.Add(newTag);

                        game.GameTags.Add(new GameTag()
                        {
                            Game = game,
                            Tag = newTag
                        });
                    }
                    else
                    {
                        game.GameTags.Add(new GameTag()
                        {
                            Game = game,
                            Tag = tag
                        });
                    }
                }
                if (game.GameTags.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                games.Add(game);
                sb.AppendLine(string.Format(SuccessfullyAddedGamesMessage, game.Name, game.Genre.Name, game.GameTags.Count));
            }
            context.Games.AddRange(games);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var usersDtos = JsonConvert.DeserializeObject<User[]>(jsonString);
            
            var users = new List<User>();
            foreach (var userDto in usersDtos)
            {
                if (!IsValid(userDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var userExists = context.Users.Any(u => u.FullName == userDto.FullName);
                if (userExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var user = new User()
                {
                    FullName = userDto.FullName,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                if (userDto.Cards.Count == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                foreach (var cardDto in userDto.Cards)
                {
                    if (!IsValid(cardDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    user.Cards.Add(new Card
                    {
                        Number = cardDto.Number,
                        Cvc = cardDto.Cvc,
                        Type = cardDto.Type
                    });                   
                }
                users.Add(user);
                sb.AppendLine(string.Format(SuccessfullyAddedUsersMessage, user.Username, user.Cards.Count));
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var sb = new StringBuilder();
            
            var xmlSerializer = new XmlSerializer(typeof(ImportPurchaseDto[]), new XmlRootAttribute("Purchases"));
            
            var purchases = new List<Purchase>();

            using (StringReader stringReader = new StringReader(xmlString))
            {
                ImportPurchaseDto[] purchaseDtos = (ImportPurchaseDto[])xmlSerializer.Deserialize(stringReader);
                
                foreach (var purchaseDto in purchaseDtos)
                {
                    if (!IsValid(purchaseDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var game = context.Games.SingleOrDefault(g => g.Name == purchaseDto.Title);

                    object typeParseObj;
                    var typeParsed = Enum.TryParse(typeof(PurchaseType), purchaseDto.Type, out typeParseObj);
                    if (!typeParsed)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var purchaseType = (PurchaseType)typeParseObj;

                    DateTime date;
                    bool isDateValid = DateTime.TryParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out date);
                   
                    if (!isDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                   
                    var card = context.Cards.SingleOrDefault(c => c.Number == purchaseDto.Card);
                    
                    if (game == null || date == null || card == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var purchase = new Purchase()
                    {
                        Game = game,
                        Type = purchaseType,
                        ProductKey = purchaseDto.Key,
                        Card = card,
                        Date = date
                    };

                    purchases.Add(purchase);
                    sb.AppendLine(string.Format(SuccessfullyAddedPurchasesMessage, game.Name, purchase.Card.User.Username));
                }
            }
            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            var result = sb.ToString().TrimEnd();
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