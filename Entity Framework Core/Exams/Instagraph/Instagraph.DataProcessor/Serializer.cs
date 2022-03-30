using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Instagraph.Data;
using Instagraph.DataProcessor.DtoModels;
using Instagraph.Models;
using Newtonsoft.Json;

namespace Instagraph.DataProcessor
{
    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var posts = context.Posts
                .Where(p => p.Comments.Count == 0)
                .OrderBy(p => p.Id)
                .Select(p => new
                {
                    Id = p.Id,
                    Picture = p.Picture.Path,   
                    User = p.User.Username
                })                
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(posts, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            return jsonString;
        }

        public static string ExportPopularUsers(InstagraphContext context)
        {
            var users = context.Users
                .Where(u => u.Posts
                            .Any(p=>p.Comments
                            .Any(c=>u.Followers
                            .Any(f=>f.FollowerId == c.UserId))))   
                .OrderBy(u => u.Id)
                .Select(u => new
                {
                    user = u.Username,
                    followers = u.Followers.Count
                })
                .ToArray();

            string jsonString = JsonConvert.SerializeObject(users, Formatting.Indented);

            return jsonString;
        }

        public static string ExportCommentsOnPosts(InstagraphContext context)
        {
            var users = context.Users
                .Select(u => new
            {
                Username = u.Username,
                Posts = u.Posts.Select(p=>p.Comments.Count).ToArray()
            })
                .ToArray();

            var outputUsers = new List<UserTopCommentDto>();
            foreach (var user in users)
            {
                int commentCount = 0;
                if (user.Posts.Any())
                {
                    commentCount = user.Posts.OrderByDescending(p => p).First();
                }

                var dto = new UserTopCommentDto()
                {
                    Username = user.Username,
                    MostComments = commentCount
                };

                outputUsers.Add(dto);
            }

            outputUsers = outputUsers.OrderByDescending(u => u.MostComments)
                .ThenBy(u => u.Username)
                .ToList();

            var xDoc = new XDocument(new XElement("users"));

            foreach (var u in outputUsers)
            {
                xDoc.Root.Add(new XElement("user",
                    new XElement("Username", u.Username),
                    new XElement("MostComments", u.MostComments)));
            }

            string xmlString = xDoc.ToString();
            return xmlString;
        }
    }
}
