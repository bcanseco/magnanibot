using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Magnanibot.Extensions
{
    public static class SocketUserMessageExtensions
    {
        public static bool IsBotQuery(this SocketUserMessage message, SocketSelfUser self, ref int argPos)
            => message.HasCharPrefix('!', ref argPos)
               || message.HasMentionPrefix(self, ref argPos)
               || message.Channel is IDMChannel;
    }
}
