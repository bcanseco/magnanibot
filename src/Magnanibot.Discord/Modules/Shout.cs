using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Magnanibot.Exceptions;

namespace Magnanibot.Modules
{
    [Group(nameof(Shout)), Alias("say", "broadcast")]
    [Summary("Sends a message to a channel.")]
    [Remarks("Example: !shout MyServer #general Hey everyone.")]
    [RequireContext(ContextType.DM | ContextType.Group)]
    [RequireOwner]
    public class Shout : Module
    {
        [Command]
        private async Task PostAsync(IGuild guild, string channelName, [Remainder] string message)
        {
            var textChannels = await guild.GetTextChannelsAsync();

            var channel = textChannels.FirstOrDefault(
                c => c.Name.IndexOf(channelName.Replace("#", string.Empty), StringComparison.OrdinalIgnoreCase) >= 0);

            if (channel == null)
                throw new BotException($"No channels found in {guild.Name} matching \"{channelName}\".");

            await channel.SendMessageAsync(string.Empty, false, new EmbedBuilder().WithDescription($"📢 {message}"));
        }
    }
}
