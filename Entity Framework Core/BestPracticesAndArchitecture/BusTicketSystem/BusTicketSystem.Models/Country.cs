namespace BusTicketSystem.Models
{
    using System.Collections.Generic;
    public class Country
    {
        private Country()
        {
            BusCompanies = new HashSet<BusCompany>();
            Towns = new HashSet<Town>();
        }
        public Country(string name)
        {
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<BusCompany> BusCompanies { get; set; }
        public ICollection<Town> Towns { get; set; }    
    }
}