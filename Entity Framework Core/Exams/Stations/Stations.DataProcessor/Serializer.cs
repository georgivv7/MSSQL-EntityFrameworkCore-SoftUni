namespace Stations.DataProcessor
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Xml;
	using System.Xml.Serialization;
	using Newtonsoft.Json;
	using Stations.Data;
	using Stations.DataProcessor.Dto.Export;
	using Stations.Models.Enums;
	using Formatting = Newtonsoft.Json.Formatting;

	public class Serializer
	{
		public static string ExportDelayedTrains(StationsDbContext context, string dateAsString)
		{
			var date = DateTime.ParseExact(dateAsString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

			var trains = context.Trains
				.Where(t => t.Trips
					.Any(tr => tr.Status == TripStatus.Delayed && tr.DepartureTime <= date))
				.Select(t => new 
				{
					t.TrainNumber,
					DelayedTrips = t.Trips
						.Where(tr => tr.Status == TripStatus.Delayed && tr.DepartureTime <= date)
						.ToArray()
				})
				.Select(t=> new TrainDto
                {
					TrainNumber = t.TrainNumber,
					DelayedTimes = t.DelayedTrips.Count(),
					MaxDelayedTime = t.DelayedTrips.Max(tr=>tr.TimeDifference).ToString()
                })
				.OrderByDescending(t=>t.DelayedTimes)
				.ThenByDescending(t=>t.MaxDelayedTime)
				.ThenBy(t=>t.TrainNumber)
				.ToArray();

			var json = JsonConvert.SerializeObject(trains, Newtonsoft.Json.Formatting.Indented);
			return json;
				
		}

		public static string ExportCardsTicket(StationsDbContext context, string cardType)
		{
			var type = Enum.Parse<CardType>(cardType);

			var cards = context.CustomerCards
				.Where(c => c.Type == type && c.BoughtTickets.Any())
				.Select(c => new CardDto
				{
					Name = c.Name,
					Type = c.Type.ToString(),
					Tickets = c.BoughtTickets.Select(t => new Dto.Export.TicketDto
					{
						OriginStation = t.Trip.OriginStation.Name,
						DestinationStation = t.Trip.DestinationStation.Name,
						DepartureTime = t.Trip.DepartureTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
					}).ToArray()
				})
				.OrderBy(c => c.Name)
				.ToArray();

			var sb = new StringBuilder();
			var serializer = new XmlSerializer(typeof(CardDto[]), new XmlRootAttribute("Cards"));
			serializer.Serialize(new StringWriter(sb), cards, new XmlSerializerNamespaces (new[] { XmlQualifiedName.Empty}));
			
			var result = sb.ToString();
			return result;
		}
	}
}