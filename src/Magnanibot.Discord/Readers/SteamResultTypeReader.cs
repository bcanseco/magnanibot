using System;
using System.Linq;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord.Commands;

namespace Magnanibot.Readers
{
    public class SteamResultTypeReader : TypeReader
    {
        public SteamResultTypeReader(SteamService service)
            => Service = service;

        private SteamService Service { get; }

        public override async Task<TypeReaderResult> Read(ICommandContext context, string gameName)
        {
            var games = (await Service.SearchAsync(gameName)).ToList();

            var game = games.FirstOrDefault(g => g.Title.Equals(gameName, StringComparison.OrdinalIgnoreCase))
                       ?? games.FirstOrDefault();

            if (game != null)
                return TypeReaderResult.FromSuccess(game);

            var errorReason = $"No Steam game was found called \"{gameName}\".";
            return TypeReaderResult.FromError(CommandError.ObjectNotFound, errorReason);
        }
    }
}
