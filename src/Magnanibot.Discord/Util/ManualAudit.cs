using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Magnanibot.Extensions;
using Magnanibot.Services;

namespace Magnanibot.Util
{
    /// <remarks>
    ///   Fill in <see cref="Guild"/> and <see cref="Channel"/>, then add:
    ///   <code>client.GuildAvailable += ManualAudit.PruneAsync;</code>
    /// </remarks>>
    public static class ManualAudit
    {
        private const ulong Guild = 0;
        private const ulong Channel = 0;

        private const string Match = "📚";
        private const string Replace = "⌛ This vote has expired.";

        // client.GuildAvailable += ManualAudit.PruneAsync;
        public static async Task PruneAsync(SocketGuild guild)
        {
            if (guild.Id != Guild) return;
            var hashBot = guild.GetChannel(Channel) as ITextChannel
                          ?? throw new NotImplementedException();

            await Logger.InfoAsync<Task>("Beginning prune...");

            var messages = new List<IMessage>();

            await hashBot
                .GetMessagesAsync(2000)
                .ForEachAsync(batch => { messages.AddRange(batch); });

            foreach (var message in messages)
            {
                if (message.Embeds.Count != 1) continue;
                if (!(message.Embeds.First().Title?.Contains(Match) ?? false)) continue;

                await ((IUserMessage)message).ModifyAsync(m =>
                    m.Embed = new EmbedBuilder()
                        .WithDescription(Replace)
                        .WithRandomColor()
                        .Build());
                await Task.Delay(TimeSpan.FromSeconds(2));
                await Logger.InfoAsync<Task>("Pruned message");
            }

            await Logger.InfoAsync<Task>("Pruning complete.");
        }
    }
}
