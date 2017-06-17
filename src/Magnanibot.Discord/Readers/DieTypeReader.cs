using System;
using System.Threading.Tasks;
using CommonBotLibrary.Services.Models;
using Discord.Commands;

namespace Magnanibot.Readers
{
    public class DieTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string sides)
        {
            try
            {
                var die = Die.Factory(sides);
                return Task.FromResult(TypeReaderResult.FromSuccess(die));
            }
            catch (Exception ex)
            {
                return Task.FromResult(TypeReaderResult
                    .FromError(CommandError.ParseFailed, ex.Message));
            }
        }
    }
}
