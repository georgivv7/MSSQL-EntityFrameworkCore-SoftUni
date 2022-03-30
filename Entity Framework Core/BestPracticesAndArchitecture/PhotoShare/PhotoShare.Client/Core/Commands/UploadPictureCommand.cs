namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class UploadPictureCommand
    {
        // UploadPicture <albumName> <pictureTitle> <pictureFilePath>
        public static string Execute(string[] data)
        {
            using (var context = new PhotoShareContext())
            {
                var album = GetAlbumByName(data[1], context);
                string pictureTitle = data[2];
                string pictureFilePath = data[3];

                context.Pictures.Add(new Picture()
                {
                    Title = pictureTitle,
                    Path = pictureFilePath,
                    Album = album
                });

                context.SaveChanges();

                return $"Picture {pictureTitle} added to {album.Name}!";
            }
            
        }

        private static Album GetAlbumByName(string name, PhotoShareContext context)
        {
            Album album = context.Albums
                .SingleOrDefault(a => a.Name == name);

            if (album == null)
            {
                throw new ArgumentException($"Album {name} not found!");
            }

            return album;
        }
    }
}
