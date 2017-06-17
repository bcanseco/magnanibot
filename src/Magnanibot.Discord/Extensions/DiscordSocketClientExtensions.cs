using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Magnanibot.Extensions
{
    public static class DiscordSocketClientExtensions
    {
        /// <summary>  
        ///   Disconnects the bot and ends the process. Does not mark watched
        ///   messages in <see cref="Services.ReactionCoordinator"/> as untracked.
        /// </summary>
        /// <remarks>
        ///   Sets status to <see cref="UserStatus.Invisible"/> first to mitigate
        ///   Discord temporarily keeping disconnected bots in the online list.
        /// </remarks>
        public static async Task TerminateAsync(this DiscordSocketClient client)
        {
            await client.SetStatusAsync(UserStatus.Invisible);
            Environment.Exit(0);
        }
    }
}
