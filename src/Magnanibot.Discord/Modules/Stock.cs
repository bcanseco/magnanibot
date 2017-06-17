using System.Threading.Tasks;
using CommonBotLibrary.Interfaces.Models;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Stock)), Alias("stocks", "nasdaq", "$")]
    [Summary("Displays a company's stock quote.")]
    public class Stock : Module
    {
        public Stock(YahooFinanceService service)
            => Service = service;

        private YahooFinanceService Service { get; }

        [Command, Alias("company", "co", "com")]
        [Summary("Gets a quote using a company name.")]
        [Remarks("Example: !stock company Alphabet, Inc.")]
        private async Task GetAsync([Remainder] SymbolBase symbol)
            => await GetAsync(symbol.ToString());

        [Command("symbol"), Alias("sym")]
        [Summary("Gets a quote using a symbol.")]
        [Remarks("Example: !stock symbol GOOG")]
        private async Task GetAsync(string symbol)
        {
            var quote = await Service.GetQuoteAsync(symbol);

            // Style the embed with an upward/downward chart depending on trends
            var stylingGroup = double.Parse(quote.Change ?? "0.0") >= 0.0
                ? (color: 0x24ae5f, thumbnail: "https://i.imgur.com/BAugCod.png" )
                : (color: 0xd25627, thumbnail: "https://i.imgur.com/ixMCiXa.png" );

            var averageDailyVolume = quote.AverageDailyVolume != null
                ? $"{int.Parse(quote.AverageDailyVolume):n0}"
                : "?";

            await EmbedAsync(new EmbedBuilder()
                .WithTitle(quote.Symbol.ToUpper())
                .WithDescription(quote.Name)
                .WithColor(new Color((uint) stylingGroup.color))
                .WithThumbnailUrl(stylingGroup.thumbnail)
                .WithFooter("NASDAQ/NYSE data provided by Yahoo Finance.")
                .WithInlineField("Bid / Ask", $"{quote.Bid ?? "?"} / {quote.Ask ?? "?"}")
                .WithInlineField("Change", $"{quote.Change ?? "?"}")
                .WithInlineField("Day's Low / High", $"{quote.DaysLow ?? "?"} / {quote.DaysHigh ?? "?"}")
                .WithInlineField("Percent Change", $"{quote.PercentChange ?? "?"}")
                .WithInlineField("Year's Low / High", $"{quote.YearLow ?? "?"} / {quote.YearHigh ?? "?"}")
                .WithInlineField("Avg. Daily Volume", $@"{averageDailyVolume}"));
        }
    }
}
