namespace Theatre.DataProcessor.ExportDto
{
    public class ExportTheatreDto
    {
        public string Name { get; set; }
        public int Halls { get; set; }  
        public decimal TotalIncome { get; set; }
        public ExportTicketDto[] Tickets { get; set; }  
    }
}
