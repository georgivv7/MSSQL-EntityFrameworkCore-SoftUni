namespace BusTicketSystem.Models
{
    using System.Collections.Generic;
    public class Town
    {
        private Town() 
        {
            Customers = new HashSet<Customer>();
            BusStations = new HashSet<BusStation>();
        }
        public Town(string name, Country country)
            :this()
        {
            Name = name;
            Country = country;
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public ICollection<BusStation> BusStations { get; set; }    
        public ICollection<Customer> Customers { get; set; }    
    }
}
