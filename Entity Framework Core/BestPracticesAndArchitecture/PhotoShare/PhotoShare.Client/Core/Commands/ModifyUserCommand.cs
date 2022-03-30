namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using System;
    using System.Linq;

    public class ModifyUserCommand
    {
        // ModifyUser <username> <property> <new value>
        // For example:
        // ModifyUser <username> Password <NewPassword>
        // ModifyUser <username> BornTown <newBornTownName>
        // ModifyUser <username> CurrentTown <newCurrentTownName>
        // !!! Cannot change username
        public static string Execute(string[] data)
        {
            string username = data[1];
            string property = data[2].ToLower();
            string newValue = data[3];

            using (var context = new PhotoShareContext())
            {
                var user = context.Users
                    .Where(u => u.Username == username)
                    .FirstOrDefault();

                if (user == null)
                {
                    throw new ArgumentException($"User {username} not found!");
                }

                string exceptionMessage = $"Value {newValue} not valid." + Environment.NewLine;
                string townError = $"Town {newValue} not found!";

                switch (property)
                {
                    case "password":
                        if (!newValue.Any(c => Char.IsLower(c))
                            || !newValue.Any(c=>char.IsDigit(c)))
                        {
                            throw new ArgumentException(exceptionMessage +
                                "Invalid Password!");
                        }

                        user.Password = newValue;
                        break;
                    case "borntown":
                        var bornTown = context.Towns
                            .Where(t => t.Name == newValue)
                            .FirstOrDefault();

                        if (bornTown == null)
                        {
                            throw new ArgumentException(exceptionMessage + townError);
                        }

                        user.BornTown = bornTown;
                        break;
                    case "currenttown":
                        var currentTown = context.Towns
                            .Where(t => t.Name == newValue)
                            .FirstOrDefault();

                        if (currentTown == null)
                        {
                            throw new ArgumentException(exceptionMessage + townError);
                        }

                        user.CurrentTown = currentTown;
                        break;
                    default:
                        throw new ArgumentException($"Property {property} not supported!");
                }

                context.SaveChanges();
                return $"User {username} {property} is {newValue}.";
            }
        }
    }
}
