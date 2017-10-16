using System;
using System.Linq;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using CommonBotLibrary.Services.Models;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Magnanibot.Readers
{
    public class SymbolTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(ICommandContext context, string companyName, IServiceProvider services)
        {
            var yahooFinanceService = services.GetService<YahooFinanceService>();
            var symbol = (await yahooFinanceService.SearchSymbolsAsync(companyName))
                .FirstOrDefault(r => r.ExchangeDisplay == "NASDAQ" || r.ExchangeDisplay == "NYSE");
            
            // If no results, assume the user entered the symbol directly
            return TypeReaderResult.FromSuccess(symbol ?? new YahooResult{Symbol = companyName});
        }
    }
}
