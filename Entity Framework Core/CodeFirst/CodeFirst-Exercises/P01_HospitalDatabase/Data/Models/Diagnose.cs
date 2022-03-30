namespace P01_HospitalDatabase.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class Diagnose
    {
        public int DiagnoseId { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }

        public int PatientId { get; set; }  
        public Patient Patient { get; set; }
    }
}
