namespace PhotoShare.Models
{
    public class Friendship
    {
        public Friendship() { }
        public Friendship(User user, User friend)
        {
            User = user;
            Friend = friend;
        }
        public int UserId { get; set; }
        public User User { get; set; }

        public int FriendId { get; set; }
        public User Friend { get; set; }

        public bool IsAccepted { get; set; }
    }
}
