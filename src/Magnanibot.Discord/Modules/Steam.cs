using System.Threading.Tasks;
using CommonBotLibrary.Services;
using CommonBotLibrary.Services.Models;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Steam)), Alias("players", "playing", "owners")]
    [Summary("Gets data for a game.")]
    [Remarks("Example: !steam mankind divided")]
    public class Steam : Module
    {
        public Steam(SteamService service)
            => Service = service;

        private SteamService Service { get; }

        [Command]
        private async Task GetAsync([Remainder] SteamResult game)
        {
            var embed = new EmbedBuilder()
                .WithTitle(game.Title)
                .WithUrl(game.Url)
                .WithImageUrl(game.ImageUrl)
                .WithColor(new Color(0x808080))
                .WithFooter($"Available on {game.Platforms}");
            
            if (game.Price != null)
            {
                var priceDisplay = game.Price.IsOnSale
                    ? $"{game.Price.Sale} (▼{game.Price.Regular})"
                    : $"{game.Price.Regular}";
                embed.WithInlineField("Price", $"{priceDisplay}");
            }

            var initialMessageTask = EmbedAsync(embed);
            
            var getPlayersTask = Service.GetCurrentPlayersAsync(game.Id);
            var getSpyDataTask = Service.GetSteamSpyDataAsync(game.Id);

            var currentPlayers = await getPlayersTask;
            var spyData = await getSpyDataTask;

            embed
                .WithDescription(spyData.Developer ?? spyData.Publisher)
                .WithInlineField("Owners", $"{spyData.Owners:n0}")
                .WithInlineField("Playing Now", $"{currentPlayers:n0}");

            await (await initialMessageTask).ModifyAsync(m =>
                m.Embed = new Optional<Embed>(embed));
        }
    }
}
