namespace P01_HospitalDatabase
{
    using System;
    using P01_HospitalDatabase.Data;
    using P01_HospitalDatabase.Data.Models;
    using P01_HospitalDatabase.Initializer;
    public class StartUp
    {
        public static void Main(string[] args)
        {
            DatabaseInitializer.ResetDatabase();

            //using (var db = new HospitalContext())
            //{
            //    db.Database.EnsureCreated();
            //}
        }
    }
}
