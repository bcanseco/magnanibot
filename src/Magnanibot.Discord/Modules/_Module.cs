using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Magnanibot.Modules
{
    public abstract class Module : ModuleBase
    {
        protected async Task<IUserMessage> EmbedAsync(EmbedBuilder builder)
            => await EmbedAsync(builder.Build());

        protected async Task<IUserMessage> EmbedAsync(Embed embed)
            => await ReplyAsync(string.Empty, false, embed);
    }
}
