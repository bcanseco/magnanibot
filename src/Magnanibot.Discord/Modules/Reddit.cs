using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;
using Magnanibot.Models;
using Magnanibot.Services;
using Category = CommonBotLibrary.Services.Models.RedditResult.PostCategory;

namespace Magnanibot.Modules
{
    [Group(nameof(Reddit)), Alias("sub", "subreddit", "leddit")]
    [Summary("Links to posts from a subreddit.")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Reddit : Module
    {
        private Reddit(RedditService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private RedditService Service { get; }
        private ReactionCoordinator Coordinator { get; }

        [Command]
        [Priority(2)]
        private async Task GetAsync(Category category)
            => await GetAsync(null, category);

        [Command, Summary("Gets posts from a subreddit listing (Hot by default).")]
        [Remarks("Example: !reddit askreddit controversial")]
        [Priority(1)]
        private async Task GetAsync(string sub = null, Category category = default)
        {
            var posts = (await Service.GetPostsAsync(sub, category, 30)).ToList();
            if (!posts.Any()) throw new BotException("No posts were found in that subreddit.");

            var (title, description, subdirectory, thumbnailUrl) = sub.ToSubredditDisplay(posts, category);

            var embeds = new List<EmbedBuilder>();
            foreach (var chunk in posts.Where(p => !p.IsStickied).ChunksOf(3))
            {
                var embed = new EmbedBuilder()
                    .WithTitle(title)
                    .WithUrl($"https://reddit.com/{subdirectory}")
                    .WithDescription(description)
                    .WithThumbnailUrl(thumbnailUrl)
                    .WithColor(new Color(0x418bbf));

                foreach (var post in chunk)
                {
                    var titleDisplay = $"{post.Title.Truncate(250)}";
                    var statsDisplay = $"⬆️️ {post.Upvotes.KDisplay()} " +
                                       $"💬 {post.CommentCount.KDisplay()} - {post.Shortlink}";

                    embed.AddField(titleDisplay, statsDisplay);
                }

                embeds.Add(embed);
            }

            var message = new PaginatedMessage(embeds, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
    }
}
