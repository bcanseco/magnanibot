using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;

namespace Magnanibot.Services
{
    public class Chatter
    {
        public Chatter(PandorabotService service)
            => Service = service;

        private PandorabotService Service { get; }
        private string ConversationId { get; set; }
        private ConcurrentDictionary<ulong, bool> AutoReplyChannels { get; }
            = new ConcurrentDictionary<ulong, bool>();

        public async Task<bool> TryRespondAsync(IUserMessage message)
        {
            if (!IsEnabled(message.Channel as ITextChannel)) return false;

            var userText = await ChatAsync(message.Content);
            await message.Channel.SendMessageAsync(userText);

            return true;
        }

        public async Task<string> ChatAsync(string message)
        {
            var response = await Service.ConverseAsync(message, ConversationId);
            ConversationId = response.ConversationId;
            return response.Reply;
        }

        /// <summary>
        ///   Toggles the auto-reply state of a channel.
        /// </summary>
        /// <param name="channel">The channel whose state will be toggled.</param>
        /// <returns>True if auto-reply is now on, false otherwise.</returns>
        public bool ToggleState(ITextChannel channel)
        {
            try
            {
                return AutoReplyChannels[channel.Id] = !AutoReplyChannels[channel.Id];
            }
            catch (KeyNotFoundException)
            {
                return AutoReplyChannels.TryAdd(channel.Id, true);
            }
        }

        /// <summary>
        ///   Returns true if auto-reply is enabled for a channel.
        /// </summary>
        private bool IsEnabled(ITextChannel channel)
        {
            try
            {
                return AutoReplyChannels[channel.Id];
            }
            catch (KeyNotFoundException)
            {
                AutoReplyChannels.TryAdd(channel.Id, false);
                return false;
            }
        }
    }
}
