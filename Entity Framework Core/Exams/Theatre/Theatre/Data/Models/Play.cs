namespace Theatre.Data.Models
{
    using Data.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Play
    {
        public Play()
        {
            Casts = new HashSet<Cast>();
            Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = @"hh\:mm\:ss")]
        [Range(typeof(TimeSpan), "01:00", "23:59")]
        public TimeSpan Duration { get; set; }

        [Required]
        [Range(typeof(float),"0.00","10.00")]
        public float Rating { get; set; }

        [Required]
        public Genre Genre { get; set; }

        [Required]
        [MaxLength(700)]
        public string Description { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        public string Screenwriter { get; set; }

        public ICollection<Cast> Casts { get; set; }
        public ICollection<Ticket> Tickets { get; set; }    
    }
}
