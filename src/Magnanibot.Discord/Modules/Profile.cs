using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Profile)), Alias("info", "whois", "discord")]
    [Summary("Gets a user's Discord profile info.")]
    [RequireContext(ContextType.Guild)]
    public class Profile : Module
    {
        [Command, Summary("Gets your own profile info.")]
        [Remarks("Example: !profile")]
        private async Task GetAsync() => await GetAsync(Context.User);

        [Command, Summary("Gets Discord info about someone on the server.")]
        [Remarks("Example: !profile @fred#1337")]
        private async Task GetAsync(IUser user)
        {
            var roleNames = string.Join(", ", Context.Guild.Roles
                .Where(r => ((IGuildUser) user).RoleIds.Contains(r.Id) && r.Name != "@everyone")
                .OrderByDescending(r => r.Position)
                .Select(r => r.Name));

            var highestRole = Context.Guild.Roles
                .OrderByDescending(r => r.Position)
                .First(r => ((IGuildUser) user).RoleIds.Contains(r.Id));

            var nickname = (user as IGuildUser)?.Nickname;

            var builder = new EmbedBuilder()
                .WithAuthor($"{user}", user.GetAvatarUrl())
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithColor(highestRole.Color)
                .WithInlineField("Account Created",
                    $"{user.CreatedAt:ddd, MMM dd, yyyy @ h:mm tt}")
                .WithInlineField("Joined Server",
                    $"{(user as IGuildUser)?.JoinedAt:ddd, MMM dd, yyyy @ h:mm tt}")
                .AddField("Roles", roleNames)
                .WithFooter(user.IsBot ? "This user is a bot." : string.Empty);

            if (nickname != null) builder.WithDescription($"**Nickname:** {nickname}");

            await EmbedAsync(builder);
        }
    }
}
