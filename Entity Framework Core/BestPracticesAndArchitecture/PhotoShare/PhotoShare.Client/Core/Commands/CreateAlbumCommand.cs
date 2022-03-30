namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Client.Utilities;
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class CreateAlbumCommand
    {
        // CreateAlbum <username> <albumTitle> <BgColor> <tag1> <tag2>...<tagN>
        public static string Execute(string[] data)
        {
            using (var context = new PhotoShareContext())
            {
                User user = GetUserByUsername(data[1],context);
                string albumTitle = data[2];
                CheckAlbumByTitle(albumTitle,context);
                Color color = ParseColor(data[3]);
                var tags = GetTags(data.Skip(4)
                    .Select(t => t.ValidateOrTransform())
                    .ToArray(),context);

                var album = new Album()
                {
                    Name = albumTitle,
                    IsPublic = true,
                    BackgroundColor = color
                };

                user.AlbumRoles.Add(new AlbumRole(user, album, Role.Owner));

                foreach (var tag in tags)
                {
                    context.AlbumTags.Add(new AlbumTag()
                    {
                        Album = album,
                        Tag = tag
                    });
                }

                context.SaveChanges();
                return $"Album {album.Name} successfully created!";
            }
            
            
        }

        private static Tag[] GetTags(string[] tagNames, PhotoShareContext context) 
        {
            if (tagNames.Length < 1)
            {
                throw new ArgumentException("Invalid tags!");
            }

            var tags = new Tag[tagNames.Length];

            for (int i = 0; i < tagNames.Length; i++)
            {
                var currentTag = context.Tags
                    .SingleOrDefault(t => t.Name == tagNames[i]);

                if (currentTag == null)
                {
                    //currentTag = new Tag()
                    //{ 
                    //    Name = tagNames[i]
                    //};
                    throw new ArgumentException("Invalid tags!");
                }

                tags[i] = currentTag; 
            }

            return tags;
        }

        private static void CheckAlbumByTitle(string albumTitle, PhotoShareContext context)
        {
            var album = context.Albums
                    .SingleOrDefault(a => a.Name == albumTitle);

            if (album != null)
            {
                throw new ArgumentException($"Album {albumTitle} exists!");
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

        private static Color ParseColor(string bgColor)
        {
            Color color;
            bool isColorAvailable = Enum.TryParse(bgColor, true, out color);

            if (!isColorAvailable)
            {
                throw new ArgumentException($"Color {bgColor} not found!");
            }

            return color;
        }
    }
}
