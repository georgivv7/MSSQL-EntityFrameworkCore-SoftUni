using _1._InitialSetup;
using System;
using System.Data.SqlClient;

namespace _2._VillainNames
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Configuration.stringConnection))
            {
                connection.Open();
                connection.ChangeDatabase("MinionsDB");

                string minionsInfo = "SELECT v.Name, COUNT(mv.MinionId) AS MinionsCount FROM Villains AS v " +
                    "JOIN MinionsVillains AS mv ON mv.VillainId = v.Id GROUP BY v.Name HAVING COUNT(mv.MinionId) >= 3 " +
                    "ORDER BY MinionsCount DESC";

                using (SqlCommand command = new SqlCommand(minionsInfo,connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader[0]} - {reader[1]}");
                        }
                    }
                }

                connection.Close();
            }
        }
    }
}
