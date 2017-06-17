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
    [Group(nameof(Anime)), Alias("a", "ani", "mal")]
    [Summary("Searches for anime.")]
    [Remarks("Example: !anime baccano")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Anime : Module
    {
        private Anime(MyAnimeListService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private MyAnimeListService Service { get; }
        private ReactionCoordinator Coordinator { get; }

        [Command]
        private async Task GetAsync([Remainder] string title)
        {
            var results = (await Service.SearchAsync(title))
                .Take(20)
                .Select(a => new EmbedBuilder()
                    .WithTitle(a.Title)
                    .WithDescription(!string.IsNullOrWhiteSpace(a.English)
                        ? a.English
                        : a.Synonyms)
                    .WithUrl(a.Url)
                    .WithImageUrl(a.Image)
                    .WithColor(new Color(0xe91e63))
                    .WithInlineField("Type", a.Type)
                    .WithInlineField("Episodes", $"{a.Episodes}")
                    .WithInlineField("Score", $"{a.Score}")
                    .WithInlineField("Status", a.Status)
                    .WithInlineField("Start date", a.StartDate)
                    .WithInlineField("End date", a.EndDate)
                    .AddField("Synopsis", a.Synopsis.Truncate(360)))
                .ToList();

            if (!results.Any())
                throw new BotException($"No anime was found matching \"{title}\".");

            var message = new PaginatedMessage(results, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
    }
}
