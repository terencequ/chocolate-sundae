using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocolateSundae.Services.Models
{
    public class UserData
    {
        public string? Username { get; set; }
        public string? FullName{ get; set; }
        public long FollowerCount { get; set; }
        public long FollowingCount { get; set; }
        public int AvailableStories { get; set; }
        public long TotalPosts { get; set; }
        public int TotalPostsInLast24Hours { get; set; }
        public int TotalReels { get; set; }
        public int TotalIGTVCount { get; set; }


        public static UserData CreateFromInstaUserInfo(InstaUserInfo info, InstaMediaList mediaInfo, InstaFullUserInfo fullInfo)
        {
            return new UserData()
            {
                Username = info.Username,
                FullName = info.FullName,
                FollowerCount = info.FollowerCount,
                FollowingCount = info.FollowingCount,
                TotalPosts = info.MediaCount,
                TotalPostsInLast24Hours = mediaInfo
                    .Where(m => m.TakenAt.AddDays(1) >= DateTime.UtcNow)
                    .Count(m => m.ProductType != "clips"),
                TotalReels = mediaInfo.Count(m => m.ProductType == "clips"),
                AvailableStories = fullInfo.UserStory.Reel.Items.Count,
                TotalIGTVCount = info.TotalIGTVVideos
            };
        }

        public override string ToString()
        {
            return 
                $"----SUMMARY----\n" +
                $"Username: {Username}\n" +
                $"Full name: {FullName}\n" +
                $"Followers: {FollowerCount}\n" +
                $"Following: {FollowingCount}\n" +
                $"Total Posts: {TotalPosts}\n" +
                $"Total Posts (Last 24 Hours): {TotalPostsInLast24Hours}\n" +
                $"Total Reels: {TotalReels}\n" +
                $"Available Stories: {AvailableStories}\n" +
                $"Total IGTV Count: {TotalIGTVCount}";
        }
    }
}
