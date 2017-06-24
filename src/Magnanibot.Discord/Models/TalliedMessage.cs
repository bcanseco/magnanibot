using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Magnanibot.Extensions;

namespace Magnanibot.Models
{
    public class TalliedMessage : InteractiveMessage
    {
        public TalliedMessage(
            int secondsRemaining,
            EmbedBuilder embedBuilder,
            Action<IDictionary<string, ISet<IGuildUser>>, IUserMessage> finishedCallback)
        {
            SecondsRemaining = secondsRemaining;
            EmbedBuilder = embedBuilder;
            FinishedCallback = finishedCallback;
            EmojiUsers = new SortedDictionary<string, ISet<IGuildUser>>
            {
                {"🇦", new HashSet<IGuildUser>()},
                {"🇧", new HashSet<IGuildUser>()},
                {"🇨", new HashSet<IGuildUser>()},
                {"🇩", new HashSet<IGuildUser>()}
            };
        }
        
        private int SecondsRemaining { get; set; }
        private EmbedBuilder EmbedBuilder { get; }
        private Action<IDictionary<string, ISet<IGuildUser>>, IUserMessage> FinishedCallback { get; }
        private IDictionary<string, ISet<IGuildUser>> EmojiUsers { get; }

        public override async Task OnSendAsync(IUserMessage sentMessage)
        {
            SentMessage = sentMessage;

            foreach (var entry in EmojiUsers)
                await sentMessage.AddReactionAsync(new Emoji(entry.Key));

            foreach (var _ in Enumerable.Range(0, SecondsRemaining / 5))
            {
                EmbedBuilder.Fields.First().Name = $"Time remaining: {SecondsRemaining} seconds";
                await SentMessage.ModifyAsync(m => m.Embed = EmbedBuilder.Build());

                SecondsRemaining -= 5;
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

            FinishedCallback.Invoke(EmojiUsers, SentMessage);
        }

        public override async Task OnReactionAddedAsync(SocketReaction reaction, IUser user)
        {
            if (EmojiUsers.TryGetValue(reaction.Emote.Name, out ISet<IGuildUser> users))
            {
                // User added a valid emoji, check to see if they already voted
                foreach (var pair in EmojiUsers)
                {
                    if (pair.Value.Contains(user as IGuildUser))
                    {
                        // Already voted, they must remove vote before choosing a different one
                        await SentMessage.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
                        return;
                    }
                }

                users.Add(user as IGuildUser);
            }
            else
            {
                // Invalid reaction, remove it
                await SentMessage.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            }
        }

        public override Task OnReactionRemovedAsync(SocketReaction reaction, IUser user)
        {
            if (EmojiUsers.TryGetValue(reaction.Emote.Name, out ISet<IGuildUser> users))
                users.Remove(user as IGuildUser);
            return Task.CompletedTask;
        }

        public override async Task OnRemoveAsync()
            => await SentMessage.ModifyAsync(m => m.Embed = new EmbedBuilder()
                .WithDescription("⌛ This vote has expired.")
                .WithRandomColor()
                .Build());

        public override Embed AsEmbed() => EmbedBuilder;
    }
}
