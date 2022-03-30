namespace VaporStore.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using VaporStore.Data.Models.Enums;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Xml.Serialization;
    public class Purchase
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        
        public PurchaseType Type { get; set; }

        [Required]
        [RegularExpression("^[A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4}$")]
        public string ProductKey { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [ForeignKey("Card")]
        public int CardId { get; set; }
        public Card Card { get; set; }

        [Required]
        [ForeignKey("Game")]
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}
