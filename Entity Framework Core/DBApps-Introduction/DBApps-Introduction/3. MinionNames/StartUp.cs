using _1._InitialSetup;
using System;
using System.Data.SqlClient;

namespace _3._MinionNames
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.stringConnection))
            {
                connection.Open();               

                string villainName = GetVillainName(villainId, connection);

                if (villainName == null)
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                }
                else
                {
                    Console.WriteLine($"Villain: {villainName}");
                    PrintNames(villainId, connection);
                }

                connection.Close();
            }
        }

        private static void PrintNames(int villainId, SqlConnection connection)
        {
            string minionNamesQuery = "SELECT m.Name, m.Age FROM Minions AS m " +
                "JOIN MinionsVillains AS mv ON mv.MinionId = m.Id WHERE MV.VillainId = @Id";

            using (SqlCommand command = new SqlCommand(minionNamesQuery,connection))
            {
                command.Parameters.AddWithValue("@Id", villainId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("(no minions)");
                    }
                    else
                    {
                        int row = 1;

                        while (reader.Read())
                        {
                            Console.WriteLine($"{row++}. {reader[0]} {reader[1]}");
                        }
                    }
                }
            }
        }

        private static string GetVillainName(int villainId, SqlConnection connection)
        {
            string nameSql = "SELECT Name FROM Villains WHERE Id = @id";

            using (SqlCommand command = new SqlCommand(nameSql,connection))
            {
                command.Parameters.AddWithValue("@id", villainId);
                return (string)command.ExecuteScalar();
            }
        }
    }
}
