using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Magnanibot.Models;

namespace Magnanibot.Services
{
    public class ReactionCoordinator
    {
        public ReactionCoordinator(DiscordSocketClient client)
        {
            BotId = client.CurrentUser.Id;

            client.ReactionAdded += OnReactionAsync;
            client.ReactionRemoved += OnReactionRemovedAsync;
            
            Task.Run(AuditMessagesAsync);
        }

        private ulong BotId { get; }
        private IDictionary<ulong, InteractiveMessage> WatchedMessages { get; }
            = new Dictionary<ulong, InteractiveMessage>();

        private async Task AuditMessagesAsync()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromDays(7));
                await Logger.InfoAsync<Task>($"Auditing {WatchedMessages.Count} watched messages.");

                foreach (var pair in WatchedMessages)
                {
                    if (pair.Value.SentMessage.CreatedAt <= DateTime.Now.AddDays(-3))
                    {
                        WatchedMessages.Remove(pair.Key);
                        await pair.Value.OnRemoveAsync();
                    }
                }

                await Logger.InfoAsync<Task>($"Finished audit. {WatchedMessages.Count} messages remain.");
            }
        }

        public async Task SendInteractiveMessageAsync(
            ICommandContext context,
            InteractiveMessage interactiveMsg)
        {
            var sentMessage = await context.Channel.SendMessageAsync(string.Empty, false, interactiveMsg);
            if ((interactiveMsg as PaginatedMessage)?.Total == 1) return;

            WatchedMessages.Add(sentMessage.Id, interactiveMsg);

            await interactiveMsg.OnSendAsync(sentMessage);
        }

        private async Task OnReactionAsync(
            Cacheable<IUserMessage, ulong> cacheable,
            ISocketMessageChannel socketMessageChannel,
            SocketReaction reaction)
        {
            var message = await cacheable.GetOrDownloadAsync();

            if (message == null) return;
            if (!reaction.User.IsSpecified) return;
            if (reaction.UserId == BotId) return;

            if (!WatchedMessages.TryGetValue(message.Id, out InteractiveMessage interactiveMsg)) return;

            await interactiveMsg.OnReactionAddedAsync(reaction, reaction.User.Value);
        }

        private async Task OnReactionRemovedAsync(
            Cacheable<IUserMessage, ulong> cacheable,
            ISocketMessageChannel socketMessageChannel,
            SocketReaction reaction)
        {
            var message = await cacheable.GetOrDownloadAsync();

            if (message == null) return;
            if (!reaction.User.IsSpecified) return;
            if (reaction.UserId == BotId) return;

            if (!WatchedMessages.TryGetValue(message.Id, out InteractiveMessage interactiveMsg)) return;

            await interactiveMsg.OnReactionRemovedAsync(reaction, reaction.User.Value);
        }
    }
}
