using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Magnanibot.Readers
{
    public class GuildTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(ICommandContext context, string guildName)
        {
            var guilds = (await context.Client.GetGuildsAsync()).ToList();
            var matchedGuild = guilds.FirstOrDefault(
                g => g.Name.IndexOf(guildName, StringComparison.OrdinalIgnoreCase) >= 0);

            if (matchedGuild != null)
                return TypeReaderResult.FromSuccess(matchedGuild);

            return TypeReaderResult.FromError(CommandError.ObjectNotFound,
                $"No servers were found matching \"{guildName}\".");
        }
    }
}
