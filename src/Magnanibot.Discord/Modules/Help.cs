using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;
using Tweetinvi.Core.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Help)), Alias("h", "ayuda", "man")]
    [Summary("Shows command information.")]
    public class Help : Module
    {
        private CommandService CommandService { get; }
        
        private Help(CommandService commandService)
            => CommandService = commandService;
        
        [Command, Summary("Displays the main help menu.")]
        [Remarks("Example: !help")]
        private async Task GetModulesAsync()
        {
            var sb = new StringBuilder("**Commands:**\n");

            CommandService.Modules.OrderBy(m => m.Name).ForEach(m =>
                sb.Append($"[!{m.Name.ToLower()}](#) - {m.Summary}\n"));

            await EmbedAsync(new EmbedBuilder()
                .WithAuthor(BotTokens.Alias, Context.Client.CurrentUser.GetAvatarUrl())
                .WithDescription(sb.AppendLine("\nUse `!help [command]` for more info").ToString())
                .WithColor(new Color(0x000001)));
        }

        [Command, Summary("Displays information for a particular command.")]
        [Remarks("Example: !help help")]
        private async Task GetCommandsAsync(ModuleInfo module)
        {
            var embed = new EmbedBuilder()
                .WithDescription($"**[{module.Name}](#)**\n{module.Summary}")
                .WithColor(new Color(0x000001));

            var aliases = module.Aliases.Skip(1).Select(s => "!" + s).ToList();
            if (aliases.Any()) embed.WithFooter($"Aliases: {string.Join(", ", aliases)}");

            var specificUsages = module.Commands
                .Where(c => c.Remarks != null && c.Summary != null).ToList();

            if (specificUsages.Any())
            {
                specificUsages.ForEach(u => embed.AddField(u.Remarks, u.Summary));
            }
            else
            {
                if (module.Remarks != null)
                {
                    var remarksArr = module.Remarks.Split(null, 2);
                    var (remarksLabel, remarksText) = (remarksArr[0], remarksArr[1]);

                    embed.AddField(remarksLabel, $"`{remarksText}`");
                }
            }

            await EmbedAsync(embed);
        }
    }
}
