using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChocolateSundae.Services.Models
{
    public class UserData
    {
        // Identification
        public string? Username { get; set; }
        public string? FullName{ get; set; }
        public DateTime Date { get; set; }
        
        // Followers
        public long FollowerCount { get; set; }
        public long FollowingCount { get; set; }
        
        // Stories
        public int AvailableStories { get; set; }
        
        // Posts
        public long TotalPosts { get; set; }
        public int TotalPostsInLast24Hours { get; set; }
        public int LikeCountRecentPost1 { get; set; }
        public int LikeCountRecentPost2 { get; set; }
        public int LikeCountRecentPost3 { get; set; }
        public int CommentCountInMostRecentPost { get; set; }
        public string? TagsInMostRecentPost { get; set; }

        // Reels
        public int TotalReels { get; set; }
        
        // IGTVs
        public int TotalIGTVCount { get; set; }


        public static UserData CreateFromInstaUserInfo(InstaUserInfo info, InstaMediaList mediaInfo, InstaFullUserInfo fullInfo)
        {
            var media = mediaInfo.OrderByDescending(m => m.TakenAt).ToList();
            var data = new UserData();
            
            // Data
            data.Username = info.Username;
            data.FullName = info.FullName;
            data.Date = DateTime.Now;
            data.FollowerCount = info.FollowerCount;
            data.FollowingCount = info.FollowingCount;
            
            // Posts
            data.TotalPosts = info.MediaCount;
            data.TotalPostsInLast24Hours = media
                .Where(m => m.TakenAt.AddDays(1) >= DateTime.UtcNow)
                .Count(m => m.ProductType != "clips");
            data.LikeCountRecentPost1 = media.Skip(0).Take(1).Sum(m => m.LikesCount);
            data.LikeCountRecentPost2 = media.Skip(1).Take(1).Sum(m => m.LikesCount);
            data.LikeCountRecentPost3 = media.Skip(2).Take(1).Sum(m => m.LikesCount);
            int.TryParse(media.FirstOrDefault()?.CommentsCount, out var commentCount);
            data.CommentCountInMostRecentPost = commentCount;
            data.TagsInMostRecentPost = string.Join(",", GetHashtagsFromText(media.FirstOrDefault()?.Caption?.Text));

            // Reels
            data.TotalReels = mediaInfo.Count(m => m.ProductType == "clips");
            
            // Stories
            data.AvailableStories = fullInfo.UserStory.Reel.Items.Count;
            
            // IGTV count
            data.TotalIGTVCount = info.TotalIGTVVideos;
            return data;
        }

        private static IEnumerable<string> GetHashtagsFromText(string? text)
        {
            if(text == null)
            {
                return Array.Empty<string>();
            }
            var regex = new Regex(@"#\w+");
            var matches = regex.Matches(text).Select(m => m.Value).AsEnumerable();
            return matches;
        }
    }
}
