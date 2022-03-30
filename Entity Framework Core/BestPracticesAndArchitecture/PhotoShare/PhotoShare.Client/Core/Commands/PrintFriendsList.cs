namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using System;
    using System.Linq;
    using System.Text;

    public class PrintFriendsListCommand 
    {
        // PrintFriendsList <username>
        public static string Execute(string[] data)
        {
            // TODO prints all friends of user with given username.
            string username = data[1];

            using (var context = new PhotoShareContext())
            {
                var user = context.Users
                    .Select(u=> new
                    {
                        u.Username,
                        AddedAsFriendBy=u.AddedAsFriendBy
                            .Select(f=>f.User.Username)
                            .ToArray(),
                        FriendsAdded = u.FriendsAdded
                            .Select(f=>f.Friend.Username)
                            .ToArray()
                    })
                    .SingleOrDefault(u => u.Username == username);

                if (user == null)
                {
                    throw new ArgumentException($"User {username} not found!");
                }

                var friends = user.AddedAsFriendBy.Concat(user.FriendsAdded);

                StringBuilder builder = new StringBuilder();

                if (friends.Any())
                {
                    builder.AppendLine("Friends: ");
                    foreach (var friend in friends)
                    {
                        builder.AppendLine($"-{friend}");   
                    }
                }
                else
                {
                    builder.Append("No friends for this user. :(");
                }

                return builder.ToString().Trim();
            }
        }
    }
}
