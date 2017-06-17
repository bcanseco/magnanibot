using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Magnanibot.Models
{
    public abstract class InteractiveMessage
    {
        public IUserMessage SentMessage { get; set; }
        protected IDictionary<string, Func<Task>> EmojiActions { get; set; }

        public virtual async Task OnReactionAddedAsync(SocketReaction reaction, IUser user)
        {
            if (EmojiActions.TryGetValue(reaction.Emote.Name, out Func<Task> action))
                await action.Invoke();
        }

        public abstract Task OnReactionRemovedAsync(SocketReaction reaction, IUser user);
        public abstract Task OnSendAsync(IUserMessage sentMessage);
        public abstract Task OnRemoveAsync();
        public abstract Embed AsEmbed();

        public static implicit operator Embed(InteractiveMessage message)
            => message.AsEmbed();
    }
}
