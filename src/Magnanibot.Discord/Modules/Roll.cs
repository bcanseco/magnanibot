using System.Threading.Tasks;
using CommonBotLibrary.Services;
using CommonBotLibrary.Services.Models;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Roll)), Alias("r", "dice", "die")]
    [Summary("Rolls a die.")]
    [Remarks("Example: !roll d20")]
    public class Roll : Module
    {
        public Roll(RandomService service)
            => Service = service;

        private RandomService Service { get; }
        
        [Command]
        private async Task GetAsync(Die die = null)
        {
            var result = Service.Roll(die);

            await EmbedAsync(new EmbedBuilder()
                .WithUserAction("🎲", Context.User, $"rolled {result}.")
                .WithColor(new Color(0xE6536F)));
        }
    }
}
