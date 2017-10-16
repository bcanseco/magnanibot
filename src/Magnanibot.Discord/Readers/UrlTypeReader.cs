using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Magnanibot.Readers
{
    public class UriTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string url, IServiceProvider services)
        {
            if (!url.Contains("://")) url = $"http://{url}";

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri link))
                return Task.FromResult(TypeReaderResult.FromSuccess(link));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                "Input could not be parsed as a url."));
        }
    }
}
