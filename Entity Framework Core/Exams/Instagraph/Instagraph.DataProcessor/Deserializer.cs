using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

using Newtonsoft.Json;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using Instagraph.Data;
using Instagraph.Models;
using Instagraph.DataProcessor.DtoModels;

namespace Instagraph.DataProcessor
{
    public class Deserializer
    {
        private static string ErrorMsg = "Error: Invalid data.";
        private static string SuccessMsg = "Successfully imported {0}.";
        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var pictures = JsonConvert.DeserializeObject<Picture[]>(jsonString).ToArray();

            var sb = new StringBuilder();

            var picList = new List<Picture>();

            foreach (var picture in pictures)
            {               
                bool isValid = String.IsNullOrEmpty(picture.Path) && picture.Size > 0;
                bool pictureExists = context.Pictures.Any(p => p.Path == picture.Path) ||
                        picList.Any(p=>p.Path == picture.Path);

                if (!isValid || pictureExists)
                {
                    sb.AppendLine(ErrorMsg);
                    continue;
                }

                picList.Add(picture);
                sb.AppendLine(String.Format(SuccessMsg, $"Picture {picture.Path}"));
            }

            context.Pictures.AddRange(picList);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            var users = JsonConvert.DeserializeObject<UserDto[]>(jsonString).ToArray();

            StringBuilder sb = new StringBuilder();

            var usersList = new List<User>();

            foreach (var userDto in users)
            {
                bool isValid = !String.IsNullOrEmpty(userDto.Username) &&
                                        userDto.Username.Length <= 30 &&
                               !String.IsNullOrEmpty(userDto.Password) &&
                                        userDto.Password.Length <= 20 &&
                               !String.IsNullOrEmpty(userDto.ProfilePicture);

                bool userExists = context.Users.Any(u => u.Username == userDto.Username) ||
                                  usersList.Any(u => u.Username == userDto.Username);

                var picture = context.Pictures.FirstOrDefault(p => p.Path == userDto.ProfilePicture);

                if (!isValid || picture == null || userExists)
                {
                    sb.AppendLine(ErrorMsg);
                    continue;
                }
                var user = Mapper.Map<User>(userDto);
                user.ProfilePicture = picture;

                usersList.Add(user);
                sb.AppendLine(String.Format(SuccessMsg, $"User {user.Username}"));  
            }

            context.Users.AddRange(usersList);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            var usersAndFollowers = JsonConvert.DeserializeObject<UserFollowerDto[]>(jsonString).ToArray();

            var sb = new StringBuilder();

            var followers = new List<UserFollower>();

            foreach (var dto in usersAndFollowers)
            {
                int? userId = context.Users.FirstOrDefault(u => u.Username == dto.User)?.Id;
                int? followerId = context.Users.FirstOrDefault(u => u.Username == dto.Follower)?.Id;

                if (userId == null || followerId == null)
                {
                    sb.AppendLine(ErrorMsg);
                    continue;   
                }

                bool alreadyFollowed = followers.Any(f => f.UserId == userId &&
                                                          f.FollowerId == followerId);
                if (alreadyFollowed)
                {
                    sb.AppendLine(ErrorMsg);
                    continue;
                }
                var userFollower = new UserFollower()
                {
                    UserId = userId.Value,
                    FollowerId = followerId.Value
                };

                followers.Add(userFollower);
                sb.AppendLine(String.Format(SuccessMsg, $"Follower {dto.Follower} to User {dto.User}"));              
            }
            context.UsersFollowers.AddRange(followers);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            var xDoc = XDocument.Parse(xmlString);

            var elements = xDoc.Root.Elements();

            var sb = new StringBuilder();

            var posts = new List<Post>();

            foreach (var element in elements)
            {
                string caption = element.Element("caption")?.Value;
                string username = element.Element("user")?.Value;
                string picturePath = element.Element("picture")?.Value;

                bool isInputValid = !String.IsNullOrWhiteSpace(caption) &&
                                    !String.IsNullOrWhiteSpace(username) &&
                                    !String.IsNullOrWhiteSpace(picturePath);

                if (!isInputValid)
                {
                    sb.AppendLine(ErrorMsg);
                    continue;
                }

                int? userId = context.Users.FirstOrDefault(u => u.Username == username)?.Id;
                int? pictureId = context.Pictures.FirstOrDefault(u => u.Path == picturePath)?.Id;

                if (userId == null || pictureId == null)
                {
                    sb.AppendLine(ErrorMsg);
                    continue;
                }

                var post = new Post()
                {
                    Caption = caption,
                    UserId = userId.Value,
                    PictureId = pictureId.Value
                };

                posts.Add(post);
                sb.AppendLine(String.Format(SuccessMsg, $"Post {caption}"));
            }

            context.Posts.AddRange(posts);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var xDoc = XDocument.Parse(xmlString);

            var elements = xDoc.Root.Elements();

            var sb = new StringBuilder();

            var comments = new List<Comment>();

            foreach (var element in elements)
            {
                string content = element.Element("content")?.Value;
                string username = element.Element("user")?.Value;
                string postIdString = element.Element("post")?.Attribute("id")?.Value;

                bool isValid = !String.IsNullOrWhiteSpace(content) &&
                               !String.IsNullOrWhiteSpace(username) &&
                               !String.IsNullOrWhiteSpace(postIdString);
                if (!isValid)
                {
                    sb.AppendLine(ErrorMsg);
                    continue;
                }

                int postId = int.Parse(postIdString);
                int? userId = context.Users.FirstOrDefault(u => u.Username == username)?.Id;
                bool postExists = context.Posts.Any(p => p.Id == postId);

                if (userId == null || !postExists)
                {
                    sb.AppendLine(ErrorMsg);
                    continue;
                }

                var comment = new Comment()
                {
                    Content = content,
                    UserId = userId.Value,
                    PostId = postId
                };

                comments.Add(comment);
                sb.AppendLine(String.Format(SuccessMsg, $"Comment {content}"));
            }

            context.Comments.AddRange(comments);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }
    }
}
