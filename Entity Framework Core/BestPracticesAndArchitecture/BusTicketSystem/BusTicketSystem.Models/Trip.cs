namespace BusTicketSystem.Models
{
    using BusTicketSystem.Models.Enumerations;
    using System;
    using System.Collections.Generic;

    public class Trip
    {
        private Trip()
        {
            this.Tickets = new HashSet<Ticket>();
        }
        public Trip(DateTime departureTime, DateTime arrivalTime, BusStation originBusStation, 
            BusStation destinationBusStation, BusCompany busCompany):this()
        {
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            OriginBusStation = originBusStation;
            DestinationBusStation = destinationBusStation;
            BusCompany = busCompany;
        }
        public int Id { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public Status? Status { get; set; }

        public int OriginBusStationId { get; set; }
        public BusStation OriginBusStation { get; set; }

        public int DestinationBusStationId { get; set; }    
        public BusStation DestinationBusStation { get; set; }

        public int BusCompanyId { get; set; }
        public BusCompany BusCompany { get; set; }

        public ICollection<Ticket> Tickets { get; set; }

    }
}