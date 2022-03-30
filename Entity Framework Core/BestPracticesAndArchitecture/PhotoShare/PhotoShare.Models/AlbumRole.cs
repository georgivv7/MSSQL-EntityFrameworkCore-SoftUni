namespace PhotoShare.Models
{
    public class AlbumRole
    {
        public AlbumRole(User user, Album album, Role permission)
        {
            this.User = user;
            this.Album = album;
            this.Role = permission;
        }
        public int UserId { get; set; }
        public User User { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; }

        public Role Role { get; set; }

        public override string ToString()
        {
            return $"{this.User.Username} - {this.Album.Name} - {this.Role}";
        }
    }
}
