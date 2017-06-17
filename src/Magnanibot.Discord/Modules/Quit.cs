using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Quit)), Alias("q", "kill", "off")]
    [Summary("Shuts down the bot.")]
    [Remarks("Example: !quit")]
    [RequireOwner]
    public class Quit : Module
    {
        [Command]
        private async Task PostAsync()
        {
            await EmbedAsync(new EmbedBuilder()
                .WithTitle("Shutting down")
                .WithDescription("Goodnight! 💤")
                .WithColor(new Color(0x4289c1)));

            await (Context.Client as DiscordSocketClient).TerminateAsync();
        } 
    }
}
