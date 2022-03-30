namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class AcceptFriendCommand
    {
        // AcceptFriend <username1> <username2>
        public static string Execute(string[] data)
        {
            
            string acceptingUsername = data[1];
            string requesterUsername = data[2]; 

            using (var context = new PhotoShareContext())
            {
                var acceptingUser = context.Users
                    .SingleOrDefault(u => u.Username == acceptingUsername);

                if (acceptingUser == null)
                {
                    throw new ArgumentException($"{acceptingUser} not found!");
                }

                var requestingUser = context.Users
                    .SingleOrDefault(u => u.Username == requesterUsername);

                if (requestingUser == null)
                {
                    throw new ArgumentException($"{requestingUser} not found!");
                }

                var friendship = context.Friendships
                    .SingleOrDefault(f => f.User.Id == requestingUser.Id && f.Friend.Id == acceptingUser.Id);

                if (friendship == null)
                {
                    throw new InvalidOperationException($"{requesterUsername} has not added {acceptingUser} as a friend");
                }

                if (friendship.IsAccepted)
                {
                    throw new InvalidOperationException($"{acceptingUsername} is already a friend to {requesterUsername}");
                }

                friendship.IsAccepted = true;

                context.SaveChanges();

                return $"{acceptingUsername} accepted {requesterUsername} as a friend";
            }
        }
    }
}
