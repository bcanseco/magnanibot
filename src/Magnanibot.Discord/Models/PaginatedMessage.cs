using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Magnanibot.Extensions;

namespace Magnanibot.Models
{
    public class PaginatedMessage : InteractiveMessage
    {
        public PaginatedMessage(IList<EmbedBuilder> pages, IUser requestor)
        {
            Pages = pages;
            RequestorId = requestor.Id;
            EmojiActions = new SortedDictionary<string, Func<Task>>
            {
                { "◀", PreviousPageAsync },
                { "▶", NextPageAsync }
            };
        }
        
        private ulong RequestorId { get; }
        private IList<EmbedBuilder> Pages { get; }
        private int CurrentPage { get; set; } = 1;
        private int Total => Pages.Count;

        private EmbedBuilder this[int i] => Pages.ElementAt(i - 1);

        private Task NextPageAsync()
        {
            if (CurrentPage != Pages.Count) CurrentPage += 1;
            return Task.CompletedTask;
        }

        private Task PreviousPageAsync()
        {
            if (CurrentPage != 1) CurrentPage -= 1;
            return Task.CompletedTask;
        }

        public override Task OnReactionRemovedAsync(SocketReaction reaction, IUser user)
            => Task.CompletedTask;

        public override async Task OnSendAsync(IUserMessage sentMessage)
        {
            SentMessage = sentMessage;
            foreach (var entry in EmojiActions.Reverse())
            {
                await sentMessage.AddReactionAsync(new Emoji(entry.Key));
            }
        }

        public override async Task OnReactionAddedAsync(SocketReaction reaction, IUser user)
        {
            var _ = SentMessage.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            if (reaction.UserId != RequestorId) return;

            await base.OnReactionAddedAsync(reaction, user); // change page
            await SentMessage.ModifyAsync(m => m.Embed = new Optional<Embed>(this)); // change embed
        }

        public override async Task OnRemoveAsync()
        {
            await SentMessage.RemoveAllReactionsAsync();

            var embed = this[CurrentPage].WithFooter("⏲ This message is no longer interactive.");
            await SentMessage.ModifyAsync(m => m.Embed = new Optional<Embed>(embed));
        }

        public override Embed AsEmbed()
            => this[CurrentPage].WithFooter($"Page {CurrentPage} of {Total}");
    }
}
