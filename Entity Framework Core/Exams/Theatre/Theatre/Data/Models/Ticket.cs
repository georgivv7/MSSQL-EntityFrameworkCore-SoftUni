namespace Theatre.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(typeof(decimal), "1.00", "100.00")]
        public decimal Price { get; set; }

        [Required]
        [Range(typeof(sbyte),"1","10")]
        public sbyte RowNumber { get; set; }

        [Required]
        [ForeignKey("Play")]
        public int PlayId { get; set; }
        public Play Play { get; set; }

        [Required]
        [ForeignKey("Theatre")]
        public int TheatreId { get; set; }
        public Theatre Theatre { get; set; }
    }
}
