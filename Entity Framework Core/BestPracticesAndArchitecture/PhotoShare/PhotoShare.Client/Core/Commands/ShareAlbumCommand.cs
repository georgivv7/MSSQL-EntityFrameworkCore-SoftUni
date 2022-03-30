namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class ShareAlbumCommand
    {
        // ShareAlbum <albumId> <username> <permission>
        // For example:
        // ShareAlbum 4 dragon321 Owner
        // ShareAlbum 4 dragon11 Viewer
        public static string Execute(string[] data)
        {
            using (var context = new PhotoShareContext())
            {
                var album = FindAlbumById(int.Parse(data[1]),context);
                var user = GetUserByUsername(data[2],context);
                var permission = ParsePermission(data[3]);

                context.AlbumRoles.Add(new AlbumRole(user, album, permission));
                context.SaveChanges();

                return $"Username {user.Username} added to album {album.Name} ({permission.ToString()})";
            }
            
        }

        private static User GetUserByUsername(string username, PhotoShareContext context)
        {
            var user = context.Users
                    .SingleOrDefault(u => u.Username == username);

            if (user == null)
            {
                throw new ArgumentException($"User {username} not found!");
            }

            return user;
        }

        private static Album FindAlbumById(int albumId, PhotoShareContext context)
        {
            var album = context.Albums
                   .SingleOrDefault(a => a.Id == albumId);

            if (album == null)
            {
                throw new ArgumentException($"Album {albumId} not found!");
            }

            return album;
        }

        private static Role ParsePermission(string name)
        {
            Role role;
            var isRoleValid = Enum.TryParse(name, true, out role);
            if (!isRoleValid)
            {
                throw new ArgumentException($"Permission must be either {"Owner"} or {"Viewer"}!");
            }

            return role;
        }
    }
}
