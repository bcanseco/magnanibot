using System;
using System.Threading.Tasks;
using CommonBotLibrary.Services.Models;
using Discord.Commands;

namespace Magnanibot.Readers
{
    public class YandexLanguageTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(new YandexLanguage(input)));
            }
            catch (Exception ex)
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, ex.Message));
            }
        }
    }
}
