namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class AddFriendCommand
    {
        // AddFriend <username1> <username2>
        public static string Execute(string[] data)
        {
            string requesterUsername = data[1];
            string addedFriendUsername = data[2];

            using (var context = new PhotoShareContext())
            {
                var requestingUser = context.Users
                    .SingleOrDefault(u => u.Username == requesterUsername);

                if (requestingUser == null)
                {
                    throw new ArgumentException($"User {requesterUsername} not found!");
                }

                var addedFriend = context.Users
                    .SingleOrDefault(u => u.Username == addedFriendUsername);

                if (addedFriend == null)
                {
                    throw new ArgumentException($"User {addedFriendUsername} not found!");
                }

                var status = context.Friendships
                    .SingleOrDefault(f =>
                        (f.Friend.Id == addedFriend.Id && f.User.Id == requestingUser.Id) ||
                        (f.Friend.Id == requestingUser.Id && f.User.Id == addedFriend.Id));

                //bool alreadyAdded = context.Friendships
                //    .Any(u => u.Friend.Id == addedFriend.Id && u.User.Id == requestingUser.Id);

                //bool accepted = context.Friendships
                //    .Any(u => u.Friend == requestingUser && u.User.Id == addedFriend.Id);

                if (status != null)
                {
                    if (!status.IsAccepted)
                    {
                        throw new InvalidOperationException($"{addedFriendUsername} has already" +
                        $"recieved a friend request from {requesterUsername}");
                    }
                    throw new InvalidOperationException($"{addedFriendUsername} is already a friend to {requesterUsername}");
                }

                context.Friendships.Add(new Friendship(requestingUser, addedFriend));
                
                context.SaveChanges();

                return $"Friend {addedFriendUsername} added to {requesterUsername}";
            }
            
        }
    }
}
