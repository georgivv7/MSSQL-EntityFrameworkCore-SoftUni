using _1._InitialSetup;
using System;
using System.Data.SqlClient;

namespace _6._RemoveVillain
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            int inputVillainId = int.Parse(Console.ReadLine());

            using (SqlConnection connection = new SqlConnection(Configuration.stringConnection))
            {
                connection.Open();

                int villainId = GetVillain(inputVillainId, connection);

                if (villainId == 0)
                {
                    Console.WriteLine("No such villain found.");
                }
                else
                {
                    int affectedRows = ReleaseMinions(villainId, connection);
                    string villainName = GetVillainName(villainId, connection);
                    DeleteVillain(villainId, connection);

                    Console.WriteLine($"{villainName} was deleted.");
                    Console.WriteLine($"{affectedRows} minions were released.");
                }
                connection.Close();
            }
        }

        private static void DeleteVillain(int villainId, SqlConnection connection)
        {
            string deleteVillainQuery = "DELETE FROM Villains WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(deleteVillainQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);

                command.ExecuteNonQuery();
            }
        }

        private static string GetVillainName(int villainId, SqlConnection connection)
        {
            string villainNameQuery = "SELECT Name FROM Villains WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(villainNameQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);

                return (string)command.ExecuteScalar();
            }
        }

        private static int ReleaseMinions(int villainId, SqlConnection connection)
        {
            string deleteQuery = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";

            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                return command.ExecuteNonQuery();
            }
        }

        private static int GetVillain(int inputVillainId, SqlConnection connection)
        {
            string villainIdQuery = "SELECT Id FROM Villains WHERE Id = @Id";

            using (SqlCommand command = new SqlCommand(villainIdQuery,connection))
            {
                command.Parameters.AddWithValue("@Id", inputVillainId);

                if (command.ExecuteScalar() == null)
                {
                    return 0;
                }

                return (int)command.ExecuteScalar();
            }
        }
    }
}
