using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Magnanibot.Modules
{
    [Group(nameof(Topic))]
    [Summary("Changes the channel topic.")]
    [Remarks("Example: !topic Talking about games")]
    [RequireContext(ContextType.Guild)]
    [RequireBotPermission(ChannelPermission.ManageChannel)]
    public class Topic : Module
    {
        [Command]
        private async Task PostAsync([Remainder] string topic = null)
            => await ((ITextChannel) Context.Channel).ModifyAsync(ch => ch.Topic = topic);
    }
}
