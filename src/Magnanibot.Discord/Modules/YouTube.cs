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

namespace Magnanibot.Modules
{
    [Group(nameof(YouTube)), Alias("y", "yt")]
    [Summary("Gets YouTube search results.")]
    [Remarks("Example: !youtube h3h3")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class YouTube : Module
    {
        private YouTube(YouTubeService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private YouTubeService Service { get; }
        private ReactionCoordinator Coordinator { get; }
        
        [Command]
        private async Task GetAsync([Remainder] string query)
        {
            var videos = (await Service.SearchAsync(query)).ToList();
            if (!videos.Any()) throw new BotException($"No videos were found for \"{query}\".");

            var embeds = new List<EmbedBuilder>();
            foreach (var chunk in videos.Take(30).ChunksOf(3))
            {
                var embed = new EmbedBuilder()
                    .WithTitle($"\"{query}\"")
                    .WithUrl($"https://www.youtube.com/results?search_query={query}")
                    .WithThumbnailUrl("https://i.imgur.com/2T2jOpZ.png") // "Player" logo
                    .WithDescription("A snippet of YouTube video results")
                    .WithColor(new Color(0xcd201f));

                foreach (var video in chunk)
                {
                    var fieldValue = $"[📺 via **{video.Snippet.ChannelTitle}** on " +
                                     $"{video.Snippet.PublishedAt:MM/dd/yyyy}](https://youtu.be/{video.Id.VideoId})";

                    embed.AddField(video.Snippet.Title, fieldValue);
                }

                embeds.Add(embed);
            }

            var message = new PaginatedMessage(embeds, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
    }
}
