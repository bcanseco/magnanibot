using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommonBotLibrary.Services.Models;
using RedditSharp.Things;

namespace Magnanibot.Extensions
{
    public static class SystemExtensions
    {
        public static string KDisplay(this int number)
            => number > 999
                ? number.ToString("#,##0,k", CultureInfo.InvariantCulture)
                : number.ToString();

        public static string Truncate(this string value, int maxChars)
            => value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";

        public static IEnumerable<IList<T>> ChunksOf<T>(this IEnumerable<T> sequence, int size)
        {
            var chunk = new List<T>(size);

            foreach (var element in sequence)
            {
                chunk.Add(element);
                if (chunk.Count == size)
                {
                    yield return chunk;
                    chunk = new List<T>(size);
                }
            }
        }

        public static bool ShouldBeSeen(this string s)
            => s != "Unknown command" && s != "A task was canceled.";

        public static (string, string, string, string) ToSubredditDisplay(
            this string sub, IEnumerable<Post> posts, RedditResult.PostCategory postCategory)
        {
            const string redditLogo = "https://i.imgur.com/inSJaN2.png";
            var category = postCategory.ToString().ToLower();

            if (sub == null)
                return ("reddit.com", $"{category} posts on the front page", category, redditLogo);
            if (sub == "all" || sub.Contains("/all"))
                return ("/r/all", $"{category} posts from all subreddits", $"r/all/{category}", redditLogo);

            var subreddit = posts.First().Subreddit;
            return ($"/r/{subreddit.Name}/{category}",
                $"{subreddit.Subscribers:n0} subscribers",
                $"r/{subreddit.Name}/{category}",
                subreddit.HeaderImage);
        }
    }
}
