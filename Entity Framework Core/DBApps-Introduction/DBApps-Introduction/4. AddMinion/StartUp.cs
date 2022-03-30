using _1._InitialSetup;
using System;
using System.Data.SqlClient;

namespace _4._AddMinion
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            string[] minionInfo = Console.ReadLine().Split();
            string[] villainInfo = Console.ReadLine().Split();

            string minionName = minionInfo[1];
            int minionAge = int.Parse(minionInfo[2]);
            string townName = minionInfo[3];

            string villainName = villainInfo[1];

            using (SqlConnection connection = new SqlConnection(Configuration.stringConnection))
            {
                connection.Open();

                int townId = GetTownId(townName, connection);
                int villainId = GetVillainId(villainName, connection);
                int minionId = InsertMinionAndGetId(minionName, minionAge, townId, connection);
                AssignMinionToVillain(villainId, minionId, connection);
                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");

                connection.Close();
            }
        }

        private static void AssignMinionToVillain(int villainId, int minionId, SqlConnection connection)
        {
            string insertMinionVillain = "INSERT INTO MinionsVillains (MinionId, VillainId) " +
                "VALUES (@minionId, @villainId)";

            using (SqlCommand command = new SqlCommand(insertMinionVillain,connection))
            {
                command.Parameters.AddWithValue("@minionId", minionId);
                command.Parameters.AddWithValue("@villainId", villainId);
                command.ExecuteNonQuery();                
            }
        }

        private static int InsertMinionAndGetId(string minionName, int minionAge, int townId, 
            SqlConnection connection)
        {
            string minionInsertQuery = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId)";

            using (SqlCommand command = new SqlCommand(minionInsertQuery,connection))
            {
                command.Parameters.AddWithValue("@name", minionName);
                command.Parameters.AddWithValue("@age", minionAge);
                command.Parameters.AddWithValue("@townId", townId);
                command.ExecuteNonQuery();
            }

            string minionGetIdQuery = "SELECT Id FROM Minions WHERE Name = @name";

            using (SqlCommand sqlCommand = new SqlCommand(minionGetIdQuery,connection))
            {
                sqlCommand.Parameters.AddWithValue("@name", minionName);
                return (int)sqlCommand.ExecuteScalar();
            }
        }

        private static int GetVillainId(string villainName, SqlConnection connection)
        {
            string villainSql = "SELECT Id FROM Villains WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(villainSql, connection))
            {
                command.Parameters.AddWithValue("@Name", villainName);

                if (command.ExecuteScalar() == null)
                {
                    InsertIntoVillains(villainName, connection);
                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }

                return (int)command.ExecuteScalar();
            }
        }

        private static void InsertIntoVillains(string villainName, SqlConnection connection)
        {
            string insertVillain = "INSERT INTO Villains (Name) VALUES (@villainName)";

            using (SqlCommand command = new SqlCommand(insertVillain, connection))
            {
                command.Parameters.AddWithValue("@villainName", villainName);
                command.ExecuteNonQuery();
            }
        }

        private static int GetTownId(string townName, SqlConnection connection)
        {
            string townSql = "SELECT Id FROM Towns WHERE Name = @Name";

            using (SqlCommand command = new SqlCommand(townSql, connection))
            {
                command.Parameters.AddWithValue("@Name", townName);

                if (command.ExecuteScalar() == null)
                {
                    InsertIntoTowns(townName, connection);
                    Console.WriteLine($"Town {townName} was added to the database.");
                }

                return (int)command.ExecuteScalar();
            }
        }

        private static void InsertIntoTowns(string townName, SqlConnection connection)
        {
            string insertTown = "INSERT INTO Towns (Name) VALUES (@townName)";

            using (SqlCommand command = new SqlCommand(insertTown,connection))
            {
                command.Parameters.AddWithValue("@townName", townName);
                command.ExecuteNonQuery();
            }
        }
    }
}
