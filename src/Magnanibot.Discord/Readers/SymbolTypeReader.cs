using System.Linq;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using CommonBotLibrary.Services.Models;
using Discord.Commands;

namespace Magnanibot.Readers
{
    public class SymbolTypeReader : TypeReader
    {
        public SymbolTypeReader(YahooFinanceService service)
            => Service = service;

        private YahooFinanceService Service { get; }

        public override async Task<TypeReaderResult> Read(ICommandContext context, string companyName)
        {
            var symbol = (await Service.SearchSymbolsAsync(companyName))
                .FirstOrDefault(r => r.ExchangeDisplay == "NASDAQ" || r.ExchangeDisplay == "NYSE");
            
            // If no results, assume the user entered the symbol directly
            return TypeReaderResult.FromSuccess(symbol ?? new YahooResult{Symbol = companyName});
        }
    }
}
