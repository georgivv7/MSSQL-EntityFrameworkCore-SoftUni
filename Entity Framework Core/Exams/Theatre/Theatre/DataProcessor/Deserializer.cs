namespace Theatre.DataProcessor
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ImportDto;
    using System.Globalization;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using System.Linq;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var plays = new List<Play>();

            var xmlSerializer = new XmlSerializer(typeof(ImportPlaysDto[]), new XmlRootAttribute("Plays"));

            using StringReader stringReader = new StringReader(xmlString);

            var playsDtos = (ImportPlaysDto[])xmlSerializer.Deserialize(stringReader);

            foreach (var playDto in playsDtos)
            {
                if (!IsValid(playDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool alreadyExists = plays.Any(p => p.Screenwriter == playDto.Screenwriter);
                if (alreadyExists)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                TimeSpan duration = TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture);

                if (duration.TotalHours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Genre genre;
                bool isGenreValid = Enum.TryParse<Genre>(playDto.Genre, out genre);
                if (!isGenreValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var play = new Play()
                {
                    Title = playDto.Title,
                    Duration = duration,
                    Rating = playDto.Rating,
                    Genre = genre,
                    Description = playDto.Description,
                    Screenwriter = playDto.Screenwriter
                };

                plays.Add(play);
                sb.AppendLine(string.Format(SuccessfulImportPlay, play.Title, play.Genre, play.Rating));
            }

            context.Plays.AddRange(plays);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var casts = new List<Cast>();

            var xmlSerializer = new XmlSerializer(typeof(ImportCastDto[]), new XmlRootAttribute("Casts"));

            using StringReader stringReader = new StringReader(xmlString);

            var castDtos = (ImportCastDto[])xmlSerializer.Deserialize(stringReader);
            if (castDtos.Any())
            {
                foreach (var castDto in castDtos)
                {
                    if (!IsValid(castDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    bool alreadyExists = casts.Any(c => c.FullName == castDto.FullName);
                    if (alreadyExists)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var play = context.Plays.FirstOrDefault(p => p.Id == castDto.PlayId);

                    var cast = new Cast()
                    {
                        FullName = castDto.FullName,
                        IsMainCharacter = castDto.IsMainCharacter,
                        PhoneNumber = castDto.PhoneNumber,
                        Play = play
                    };

                    string characterType = castDto.IsMainCharacter == true ? "main" : "lesser";

                    casts.Add(cast);
                    sb.AppendLine(string.Format(SuccessfulImportActor, cast.FullName, characterType));
                }
            }
            

            context.Casts.AddRange(casts);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var theatreDtos = JsonConvert.DeserializeObject<ImportTheatresDto[]>(jsonString);

            var theatres = new List<Theatre>();

            var tickets = new List<Ticket>();

            if (theatreDtos.Any())
            {
                foreach (var theatreDto in theatreDtos)
                {
                    if (!IsValid(theatreDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    var alreadyExists = theatres.Any(t => t.Name == theatreDto.Name);
                    if (alreadyExists)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var theatre = new Theatre()
                    {
                        Name = theatreDto.Name,
                        NumberOfHalls = theatreDto.NumberOfHalls,
                        Director = theatreDto.Director
                    };
                    if (theatreDto.Tickets.Any())
                    {
                        foreach (var ticketDto in theatreDto.Tickets)
                        {
                            if (!IsValid(ticketDto))
                            {
                                sb.AppendLine(ErrorMessage);
                                continue;
                            }

                            bool alreadyExist = tickets.Any(t => t.Price == ticketDto.Price && t.RowNumber == ticketDto.RowNumber
                                                           && t.PlayId == ticketDto.PlayId);
                            if (alreadyExist)
                            {
                                sb.AppendLine(ErrorMessage);
                                continue;
                            }

                            var play = context.Plays.FirstOrDefault(p => p.Id == ticketDto.PlayId);

                            var ticket = new Ticket()
                            {
                                Price = ticketDto.Price,
                                RowNumber = ticketDto.RowNumber,
                                Play = play
                            };

                            tickets.Add(ticket);
                            theatre.Tickets.Add(ticket);
                        }
                    }

                    theatres.Add(theatre);
                    sb.AppendLine(string.Format(SuccessfulImportTheatre, theatre.Name, theatre.Tickets.Count));
                }
            }
            

            context.Theatres.AddRange(theatres);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
