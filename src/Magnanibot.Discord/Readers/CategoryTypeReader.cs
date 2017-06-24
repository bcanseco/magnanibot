using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonBotLibrary.Services.Models;
using Discord.Commands;

namespace Magnanibot.Readers
{
    public class CategoryTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            // Strip symbols and spaces
            input = new Regex("[^a-zA-Z0-9]").Replace(input, string.Empty);

            var category = Enum.GetNames(typeof(OpenTriviaDbCategory))
                .FirstOrDefault(c => c.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0);

            if (category != null)
            {
                return Task.FromResult(TypeReaderResult.FromSuccess(
                    Enum.Parse(typeof(OpenTriviaDbCategory), category)));
            }

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                "Input could not be parsed as a category."));
        }
    }
}
