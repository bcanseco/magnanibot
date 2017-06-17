using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Magnanibot.Modules
{
    [Group(nameof(Game))]
    [Summary("Shows the bot 'playing' a game.")]
    [Remarks("Example: !game Using CPU cycles")]
    public class Game : Module
    {
        [Command]
        private async Task PostAsync([Remainder] string gameName = null)
            => await ((DiscordSocketClient) Context.Client).SetGameAsync(gameName);
    }
}
