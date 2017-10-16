using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Magnanibot.Readers
{
    public class ModuleInfoTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string moduleName, IServiceProvider services)
        {
            var commandService = services.GetService<CommandService>();
            var modules = commandService.Modules as IList<ModuleInfo> ?? commandService.Modules.ToList();
            moduleName = moduleName.Replace("!", string.Empty);

            var matchedModule =
                modules.FirstOrDefault(m => m.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                ?? modules.FirstOrDefault(m => m.Aliases.Contains(moduleName, StringComparer.OrdinalIgnoreCase));

            if (matchedModule != null)
                return Task.FromResult(TypeReaderResult.FromSuccess(matchedModule));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound,
                $"No commands were found matching \"{moduleName}\"."));
        }
    }
}
