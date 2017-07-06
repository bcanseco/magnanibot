using System;
using System.Threading.Tasks;
using CommonBotLibrary.Interfaces.Models;
using CommonBotLibrary.Services;
using CommonBotLibrary.Services.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Magnanibot.Readers;
using Magnanibot.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Magnanibot.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///   Adds <see cref="DiscordSocketClient"/> as a singleton with
        ///   preconfigured options to the <see cref="IServiceCollection"/>.
        /// </summary>
        public static IServiceCollection AddSocketClient(
            this IServiceCollection collection)
        {
            return collection.AddSingleton(new DiscordSocketClient(
                new DiscordSocketConfig
                {
                    AlwaysDownloadUsers = true,
                    DefaultRetryMode = RetryMode.RetryRatelimit
                }));
        }

        /// <summary>
        ///   Creates a <see cref="CommandService"/> with type readers and
        ///   other services, then adds it to the <see cref="IServiceCollection"/>.
        /// </summary>
        public static IServiceCollection AddCommandService(
            this IServiceCollection collection)
        {
            var service = new CommandService();
            var steamService = new SteamService();
            var yahooFinanceService = new YahooFinanceService();

            collection.AddSingleton(service);
            collection.AddSingleton(steamService);
            collection.AddSingleton(yahooFinanceService);
            collection.AddSingleton<NCalcService>();
            collection.AddSingleton<RandomService>();
            collection.AddSingleton<OpenWeatherMapService>();
            collection.AddSingleton<MyAnimeListService>();
            collection.AddSingleton<UrbanDictionaryService>();
            collection.AddSingleton<OmdbService>();
            collection.AddSingleton<TwitterService>();
            collection.AddSingleton<RedditService>();
            collection.AddSingleton<GoogleSearchService>();
            collection.AddSingleton<YouTubeService>();
            collection.AddSingleton<OpenTriviaDbService>();
            collection.AddSingleton<ImgurService>();
            collection.AddSingleton<GithubService>();
            collection.AddSingleton<YandexTranslateService>();
            collection.AddSingleton<GoogleVisionService>();
            collection.AddSingleton<WatsonPersonalityService>();

            service.AddTypeReader<ModuleInfo>(new ModuleInfoTypeReader(service));
            service.AddTypeReader<IGuild>(new GuildTypeReader());
            service.AddTypeReader<Uri>(new UriTypeReader());
            service.AddTypeReader<Die>(new DieTypeReader());
            service.AddTypeReader<SymbolBase>(new SymbolTypeReader(yahooFinanceService));
            service.AddTypeReader<SteamResult>(new SteamResultTypeReader(steamService));
            service.AddTypeReader<OpenTriviaDbCategory>(new CategoryTypeReader());
            service.AddTypeReader<YandexLanguage>(new YandexLanguageTypeReader());

            return collection;
        }

        /// <summary>
        ///   Adds and initializes project-specific services and returns
        ///   the result of <see langword="BuildServiceProvider()"/>.
        /// </summary>
        public static async Task<IServiceProvider> BuildBotProviderAsync(
            this IServiceCollection collection)
        {
            collection.AddSingleton<Logger>();
            collection.AddSingleton<CommandHandler>();
            collection.AddSingleton<ReactionCoordinator>();

            var provider = collection.BuildServiceProvider();

            provider.GetRequiredService<Logger>();

            return await provider
                .GetRequiredService<CommandHandler>()
                .InitializeAsync(provider);
        }
    }
}
