namespace Stations.DataProcessor.Dto.Export
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TrainDto
    {
        [Required]
        [MaxLength(10)]
        public string TrainNumber { get; set; }
        public int DelayedTimes { get; set; }
        public string MaxDelayedTime { get; set; }  
    }
}
