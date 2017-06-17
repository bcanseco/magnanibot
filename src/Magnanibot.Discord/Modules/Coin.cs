using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Coin)), Alias("coinflip", "flip", "f")]
    [Summary("Flips a coin.")]
    [Remarks("Example: !coin")]
    public class Coin : Module
    {
        public Coin(RandomService service)
            => Service = service;

        private RandomService Service { get; }

        [Command]
        private async Task GetAsync()
        {
            var (color, emoji, result) = Service.FlipCoin() 
                ? (0xd79e84, "🐵", "heads")
                : (0xbf6952, "🐒", "tails");

            await EmbedAsync(new EmbedBuilder()
                .WithUserAction(emoji, Context.User, $"got {result}.")
                .WithColor(new Color((uint) color)));
        }
    }
}
