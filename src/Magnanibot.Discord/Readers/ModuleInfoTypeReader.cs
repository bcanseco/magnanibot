using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;

namespace Magnanibot.Readers
{
    public class ModuleInfoTypeReader : TypeReader
    {
        public ModuleInfoTypeReader(CommandService service)
            => Service = service;

        private CommandService Service { get; }

        public override Task<TypeReaderResult> Read(ICommandContext context, string moduleName)
        {
            var modules = Service.Modules as IList<ModuleInfo> ?? Service.Modules.ToList();
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
