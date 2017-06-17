﻿using System;
using System.Threading.Tasks;
using CommonBotLibrary;
using Discord;
using Discord.WebSocket;
using Magnanibot.Context;
using Magnanibot.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Magnanibot
{
    public class Magnanibot
    {
        private static void Main()
            => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var services = await ConfigureServicesAsync();
            var client = services.GetRequiredService<DiscordSocketClient>();

            await Tokens.LoadAsync<BotTokens>();
            await client.LoginAsync(TokenType.Bot, BotTokens.Discord);
            await client.StartAsync();

            await Task.Delay(-1); // block task so bot stays running
        }

        private static async Task<IServiceProvider> ConfigureServicesAsync()
            => await new ServiceCollection()
                .AddSocketClient()
                .AddCommandService()
                .AddDbContext<BotContext>()
                .BuildBotProviderAsync();
    }
}
