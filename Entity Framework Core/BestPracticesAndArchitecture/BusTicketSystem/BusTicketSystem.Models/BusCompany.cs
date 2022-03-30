using System;
using System.Collections.Generic;

namespace BusTicketSystem.Models
{
    public class BusCompany 
    {
        public const byte MinRating = 1;
        public const byte MaxRating = 10;

        private readonly string InvalidRationgExceptionMessage = $"Rating must be between {MinRating} and {MaxRating}";
        
        private double? rating;
        private BusCompany()
        {
            this.Trips = new HashSet<Trip>();
            this.Reviews = new HashSet<Review>();
        }
        public BusCompany(string name, Country country)
        {
            Name = name;
            Country = country;
        }
        public int Id { get; set; } 
        public string Name { get; set; }
        public double? Rating 
        {
            get { return this.rating; }
            set 
            {
                if (value < MinRating || value > MaxRating)
                {
                    throw new ArgumentException(InvalidRationgExceptionMessage);
                }

                this.rating = value;
            }
        }
        public int CountryId { get; set; }
        public Country Country { get; set; }

        public ICollection<Trip> Trips { get; set; }
        public ICollection<Review> Reviews { get; set; }    
    }
}
