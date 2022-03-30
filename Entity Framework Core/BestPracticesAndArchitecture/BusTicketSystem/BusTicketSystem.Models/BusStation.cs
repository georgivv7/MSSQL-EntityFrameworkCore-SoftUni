using System.Collections.Generic;

namespace BusTicketSystem.Models
{
    public class BusStation
    {
        private BusStation()
        {
            StartingTrips = new HashSet<Trip>();
            ArrivingTrips = new HashSet<Trip>();
        }
        public BusStation(string name, Town town)
            :this()
        {
            Name = name;
            Town = town;
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public int TownId { get; set; }
        public Town Town { get; set; }

        public ICollection<Trip> StartingTrips { get; set; }
        public ICollection<Trip> ArrivingTrips { get; set; }    
    }
}