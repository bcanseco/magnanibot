using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Magnanibot.Modules
{
    [Group(nameof(Rename)), Alias("name", "nick", "rn", "n")]
    [Summary("Changes nicknames.")]
    [RequireContext(ContextType.Guild)]
    public class Rename : Module
    {
        [Command, Summary("Renames the bot.")]
        [Remarks("Example: !rename Skynet")]
        [RequireBotPermission(GuildPermission.ChangeNickname)]
        [Priority(1)]
        private async Task PostAsync([Remainder] string name = null)
        {
            var self = await Context.Guild.GetCurrentUserAsync();
            await self.ModifyAsync(u => u.Nickname = name ?? self.Username);
        }
        
        [Command, Summary("Renames a user.")]
        [Remarks("Example: !rename @John#1337 Clown")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [Priority(2)]
        private async Task PostAsync(IGuildUser user, [Remainder] string name = null)
            => await user.ModifyAsync(u => u.Nickname = name ?? user.Username);
    }
}
