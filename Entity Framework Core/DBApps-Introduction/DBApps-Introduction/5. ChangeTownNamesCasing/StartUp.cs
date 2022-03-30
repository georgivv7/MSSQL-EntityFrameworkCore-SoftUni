using _1._InitialSetup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _5._ChangeTownNamesCasing
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            string countryName = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(Configuration.stringConnection))
            {
                connection.Open();

                int countryId = GetCountryId(countryName, connection);
                if (countryId == 0)
                {
                    Console.WriteLine("No town names were affected.");
                }
                else
                {
                    int affectedRows = UpdateNames(countryId, connection);
                    List<string> names = GetNames(countryId, connection);
                    Console.WriteLine($"{affectedRows} town names were affected.");
                    Console.WriteLine($"[{String.Join(", ",names)}]");
                }

                connection.Close();
            }
        }

        private static List<string> GetNames(int countryId, SqlConnection connection)
        {
            List<string> names = new List<string>();

            string namesQuery = "SELECT Name FROM Towns WHERE CountryCode = @countryId";

            using (SqlCommand command = new SqlCommand(namesQuery,connection))
            {
                command.Parameters.AddWithValue("@countryId", countryId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        names.Add((string)reader[0]);
                    }
                }
            }
            return names;
        }

        private static int UpdateNames(int countryId, SqlConnection connection)
        {
            string updateNamesQuery = "UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = @countryId";

            using (SqlCommand command = new SqlCommand(updateNamesQuery,connection))
            {
                command.Parameters.AddWithValue("@countryId", countryId);
                return command.ExecuteNonQuery();
            }
        }

        private static int GetCountryId(string countryName, SqlConnection connection)
        {
            string countryIdSql = "SELECT Id FROM Countries WHERE Name = @name";

            using (SqlCommand command = new SqlCommand(countryIdSql,connection))
            {
                command.Parameters.AddWithValue("@name", countryName);

                if (command.ExecuteScalar() == null)
                {
                    return 0;
                }
                return (int)command.ExecuteScalar();
            }
        }
    }
}
