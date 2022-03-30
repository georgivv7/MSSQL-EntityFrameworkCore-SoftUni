namespace BusTicketSystem.Client.Commands
{
    using BusTicketSystem.Data;
    using System;
    using System.Linq;
    using System.Text;

    public class PrintInfoCommand
    {
        private const string InvalidStationMessage = "The station does not exist in the database.";
        public static string Execute(int busStationId)
        {
            using (var context = new BusTicketSystemContext())
            {
                var station = context.BusStations
                    .SingleOrDefault(bs => bs.Id == busStationId);

                if (station == null)
                {
                    throw new ArgumentException(InvalidStationMessage);
                }

                var sb = new StringBuilder();

                sb.AppendLine(station.Name + " " + station.Town);
                sb.AppendLine("Arrivals:");

                foreach (var arrival in station.ArrivingTrips)
                {                   
                    sb.AppendLine($"From {arrival.OriginBusStation.Name} | Arrive at: {arrival.ArrivalTime} | " +
                        $"Status: {arrival.Status.Value}");
                }
                sb.AppendLine("Departures:");
                foreach (var departure in station.StartingTrips)
                {
                    
                    sb.AppendLine($"To {departure.DestinationBusStation.Name} | Depart at: {departure.DepartureTime} | " +
                        $"Status: {departure.Status.Value}");
                }

                return sb.ToString().Trim();
            }
            
        }
    }
}
