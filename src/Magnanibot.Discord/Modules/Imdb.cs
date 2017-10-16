using System.Linq;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using CommonBotLibrary.Services.Models;
using Discord;
using Discord.Commands;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;
using Magnanibot.Models;
using Magnanibot.Services;

namespace Magnanibot.Modules
{
    [Group(nameof(Imdb)), Alias("movie", "movies", "tv", "television")]
    [Summary("Gets information about a movie or TV show.")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Imdb : Module
    {
        private Imdb(OmdbService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private OmdbService Service { get; }
        private ReactionCoordinator Coordinator { get; }

        private async Task GetAsync(string title, OmdbType type = default, int? year = null)
        {
            var media = await Service.DirectAsync(title, type, year);
            
            var embed = new EmbedBuilder()
                .WithTitle($"{media.Title} ({(media.Rated != "N/A" ? media.Rated : "Unrated")})")
                .WithUrl($"http://www.imdb.com/title/{media.ImdbId}")
                .WithDescription(media.Actors)
                .WithColor(new Color(0x71368a))
                .WithInlineField("Released", media.Released)
                .WithInlineField("Runtime", media.Runtime)
                .WithInlineField("Genre", media.Genre)
                .WithInlineField("Metascore", media.Metascore)
                .WithInlineField("Language", media.Language)
                .WithInlineField("Director", media.Director)
                .WithFooter(media.Awards != "N/A" ? media.Awards : null)
                .WithThumbnailUrl(media.Poster != "N/A", media.Poster)
                .AddField("Plot", media.Plot.Truncate(360));

            await EmbedAsync(embed);
        }

        private async Task GetMultipleAsync(string title, OmdbType type = default, int? year = null)
        {
            var media = (await Service.SearchAsync(title, type, year))
                .Select(m => new EmbedBuilder()
                    .WithTitle($"{m.Title} ({m.Year})")
                    .WithDescription($"`!imdb {m.Year} {m.Type.ToString().ToLower()} {m.Title}`")
                    .WithUrl($"http://www.imdb.com/title/{m.ImdbId}")
                    .WithImageUrl(m.Poster != "N/A", m.Poster)
                    .WithColor(new Color(0x432052)))
                .ToList();

            if (!media.Any())
                throw new BotException($"No media was found matching \"{title}\".");

            var message = new PaginatedMessage(media, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }

        #region Commands
        [Command, Summary("Gets the most relevant result for a title.")]
        [Remarks("Example: !imdb batman")]
        [Priority(1)]
        private async Task GetAsync([Remainder] string title)
            => await GetAsync(title, default);

        [Command, Summary("Gets the most relevant [movie/series] by title.")]
        [Remarks("Example: !imdb series batman")]
        [Priority(2)]
        private async Task GetAsync(OmdbType type, [Remainder] string title)
            => await GetAsync(title, type);

        [Command, Summary("Gets the most relevant [movie/series] by year and title.")]
        [Remarks("Example: !imdb 2008 movie batman")]
        [Priority(3)]
        private async Task GetAsync(int year, OmdbType type, [Remainder] string title)
            => await GetAsync(title, type, year);

        [Command("search")]
        [Summary("Searches for relevant results by a title.")]
        [Remarks("Example: !imdb search batman")]
        [Priority(4)]
        private async Task GetMultipleAsync([Remainder] string title)
            => await GetMultipleAsync(title, default);

        [Command("search")]
        [Summary("Searches for relevant [movie/series] by title.")]
        [Remarks("Example: !imdb search series batman")]
        [Priority(5)]
        private async Task GetMultipleAsync(OmdbType type, [Remainder] string title)
            => await GetMultipleAsync(title, type);

        [Command("search")]
        [Summary("Searches for relevant [movie/series] by year and title.")]
        [Remarks("Example: !imdb search 2008 movie batman")]
        [Priority(6)]
        private async Task GetMultipleAsync(int year, OmdbType type, [Remainder] string title)
            => await GetMultipleAsync(title, type, year);
        #endregion
    }
}
