using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;
using Magnanibot.Models;
using Magnanibot.Services;
using IUser = Discord.IUser;

namespace Magnanibot.Modules
{
    [Group(nameof(Twitter)), Alias("t", "tweets", "tweet", "@")]
    [Summary("Gets someone's latest tweets.")]
    [Remarks("Example: !twitter dril")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Twitter : Module
    {
        private Twitter(TwitterService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private TwitterService Service { get; }
        private ReactionCoordinator Coordinator { get; }
        
        [Command]
        [Priority(1)]
        private async Task GetAsync(string handle)
        {
            var tweets = (await Service.GetRecentTweetsAsync(handle, false))
                .Take(20)
                .Select(t => new EmbedBuilder()
                    .WithTitle($"@{t.CreatedBy.ScreenName}")
                    .WithUrl($"https://twitter.com/{t.CreatedBy.ScreenName}")
                    .WithThumbnailUrl($"{t.CreatedBy.ProfileImageUrl400x400}")
                    .WithColor(new Color(0x1da1f2))
                    .WithDescription($"{t.CreatedBy.FollowersCount:n0} followers")
                    .AddField($"🗨 {t.CreatedBy.Name}", WebUtility.HtmlDecode(t.Text))
                    .WithInlineField("Stats", $"{t.FavoriteCount:n0} 💙 " +
                                              $"{t.RetweetCount:n0} 🔁")
                    .WithInlineField("Timestamp", $"[{t.CreatedAt:g}]({t.Url})")
                    .WithNullableImageUrl(t.Entities.Medias.FirstOrDefault(m =>
                        m.MediaType == "photo")?.MediaURLHttps))
                .ToList();

            var message = new PaginatedMessage(tweets, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
        
        [Command]
        [Priority(2)]
        private async Task GetAsync(IUser user)
            => await GetAsync((user as IGuildUser)?.Nickname ?? user.Username);
    }
}
