namespace BusTicketSystem.Models
{
    using System;
    public class Review
    {
        public const byte MinRange = 1;
        public const byte MaxRange = 10;

        private readonly string InvalidGradeExceptionMessage = $"Grade value must be in the range [{MinRange},{MaxRange}].";
        
        private double grade;
        private Review() { }
        public Review(string content, double grade, BusCompany busCompany, Customer customer)
        {
            Content = content;
            Grade = grade;
            BusCompany = busCompany;
            Customer = customer;
        }
        public int Id { get; set; }
        public string Content { get; set; }
        public double Grade 
        {
            get { return this.grade; }
            set 
            {
                if (value < MinRange || value > MaxRange)
                {
                    throw new ArgumentException(InvalidGradeExceptionMessage);
                }

                this.grade = value;
            } 
        }
        public int BusCompanyId { get; set; }
        public BusCompany BusCompany { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime PublishDate { get; set; }   
    }
}
