using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Magnanibot.Extensions;

namespace Magnanibot.Services
{
    public class CommandHandler
    {
        public CommandHandler(
            IServiceProvider provider, 
            DiscordSocketClient discord, 
            CommandService commands, 
            Chatter chatter)
        {
            (Discord, Commands, Provider, Chatter) = (discord, commands, provider, chatter);
            Discord.MessageReceived += OnMessageAsync;
        }

        private DiscordSocketClient Discord { get; }
        private CommandService Commands { get; }
        private Chatter Chatter { get; }
        private IServiceProvider Provider { get; set; }

        public async Task<IServiceProvider> InitializeAsync(IServiceProvider provider)
        {
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
            return Provider = provider;
        }

        private async Task OnMessageAsync(SocketMessage rawMessage)
        {
            var argPos = 0;

            var message = rawMessage as SocketUserMessage;
            if (message?.Source != MessageSource.User) return;

            if (!message.IsBotQuery(Discord.CurrentUser, ref argPos))
            {
                await Chatter.TryRespondAsync(message);
                return;
            }

            var context = new SocketCommandContext(Discord, message);

            /* This runs all commands asynchronously regardless of RunMode (for exceptions)
               The task is not awaited to avoid blocking the gateway. */
            var _ = Task.Run(async () =>
            {
                var result = await Commands.ExecuteAsync(context, argPos, Provider);

                if (result.Error.HasValue && result.ErrorReason.ShouldBeSeen())
                    await context.Channel.SendMessageAsync(string.Empty, false, new EmbedBuilder()
                        .WithTitle("An error occurred")
                        .WithDescription("Please use `!help` for command info and examples.")
                        .WithFooter($"❌ {result.Error}: {result.ErrorReason}")
                        .WithColor(new Color(0xFF0000)));
            });
        }
    }
}
