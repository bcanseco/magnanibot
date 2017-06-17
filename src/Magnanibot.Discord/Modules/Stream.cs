using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Magnanibot.Exceptions;

namespace Magnanibot.Modules
{
    [Group(nameof(Stream)), Alias("twitch", "streaming")]
    [Summary("Shows the bot streaming a game.")]
    [Remarks("Example: !stream twitch.tv/siglemic Super Mario 64")]
    public class Stream : Module
    {
        [Command]
        private async Task PostAsync(Uri streamLink, [Remainder] string gameName)
        {
            if (streamLink.Host != "twitch.tv")
                throw new BotException("Url host must be twitch.tv to set a stream.");

            await ((DiscordSocketClient) Context.Client)
                .SetGameAsync(gameName, streamLink.AbsoluteUri, StreamType.Twitch);
        }
    }
}
