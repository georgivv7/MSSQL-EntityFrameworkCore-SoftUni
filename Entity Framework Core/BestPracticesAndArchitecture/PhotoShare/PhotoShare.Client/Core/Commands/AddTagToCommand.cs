namespace PhotoShare.Client.Core.Commands
{
    using PhotoShare.Data;
    using PhotoShare.Models;
    using System;
    using System.Linq;

    public class AddTagToCommand 
    {
        // AddTagTo <albumName> <tag>
        public static string Execute(string[] data)
        {
            string albumName = data[1];
            string tagName = data[2];

            using (var context = new PhotoShareContext())
            {
                var album = context.Albums
                    .SingleOrDefault(a => a.Name == albumName);

                var tag = context.Tags
                    .SingleOrDefault(t => t.Name == tagName);
                
                if (album == null || tag == null)
                {
                    throw new ArgumentException("Either tag or album do not exist!");
                }

                var albumTag = new AlbumTag();
                albumTag.Album = album;
                albumTag.Tag = tag;

                context.AlbumTags.Add(albumTag);
                context.SaveChanges();

                return $"Tag {tagName} added to {albumName}";
            }
        }
    }
}
