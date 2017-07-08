using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;
using Magnanibot.Services;

namespace Magnanibot.Modules
{
    [Group(nameof(Chat)), Alias("clever", "chatbot", "speak", "ask", "talk")]
    [Summary("Chats with a Cleverbot-like AI.")]
    [RequireContext(ContextType.Guild)]
    public class Chat : Module
    {
        private Chat(Chatter service)
            => Service = service;

        private Chatter Service { get; }

        [Command, Summary("Continues the conversation.")]
        [Remarks("Example: !chat What's up?")]
        [Priority(1)]
        private async Task GetAsync([Remainder] string message)
        {
            var response = await Service.ChatAsync(message);
            await ReplyAsync(response);
        }

        [Command("toggle"), Summary("Toggles auto-replies in the current channel.")]
        [Remarks("Example: !chat toggle")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Priority(2)]
        private async Task PutAsync()
            => await PutAsync(Context.Channel as ITextChannel);

        [Command("toggle"), Summary("Toggles auto-replies in a specific channel.")]
        [Remarks("Example: !chat toggle #general")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Priority(2)]
        private async Task PutAsync(ITextChannel channel)
        {
            var (state, color) = Service.ToggleState(channel)
                ? ("on", new Color(0xffd983))
                : ("off", new Color(0xc9d3da));

            await EmbedAsync(new EmbedBuilder()
                .WithColor(color)
                .WithUserAction("💡", Context.User,
                    $"auto-replies are now *{state}* for `#{channel}`.", true));
        }
    }
}
