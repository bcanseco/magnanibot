using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;

namespace Magnanibot.Services
{
    public class Logger
    {
        public Logger(DiscordSocketClient discord, CommandService commands)
        {
            Console.Title = BotTokens.Alias;
            Console.CancelKeyPress += async (s, a) => { await discord.TerminateAsync(); };

            discord.Log += OnLogAsync;
            commands.Log += OnLogAsync;
        }
        
        private static IDictionary<LogSeverity, ConsoleColor> SeverityStyles { get; }
            = new Dictionary<LogSeverity, ConsoleColor>
            {
                {LogSeverity.Critical, ConsoleColor.DarkMagenta},
                {LogSeverity.Error, ConsoleColor.DarkRed},
                {LogSeverity.Warning, ConsoleColor.DarkYellow},
                {LogSeverity.Debug, ConsoleColor.Yellow},
                {LogSeverity.Info, ConsoleColor.DarkCyan},
                {LogSeverity.Verbose, ConsoleColor.White}
            };

        private static Task OnLogAsync(LogMessage message)
            => LogAsync(message.Severity, message.Source, message.Message, message.Exception);

        public static Task LogAsync(LogSeverity severity, string source, string message, Exception exception = null)
        {
            void WriteWithColor(string text, ConsoleColor color)
            {
                Console.ForegroundColor = color;
                Console.Write(text);
            }

            var ex = exception?.InnerException;
            if (ex is BotException || ex is RateLimitedException) return Task.CompletedTask;

            lock (SeverityStyles)
            {
                WriteWithColor($"{severity.ToString().ToUpper(),7} ", SeverityStyles[severity]);
                WriteWithColor("[", ConsoleColor.White);
                WriteWithColor($"{DateTime.Now}", ConsoleColor.Gray);
                WriteWithColor("]", ConsoleColor.White);
                WriteWithColor($" {source,-12}", ConsoleColor.DarkGreen);
                WriteWithColor(" - ", ConsoleColor.White);
                WriteWithColor($"{message ?? exception?.ToString()}\n", ConsoleColor.Gray);
            }
            
            return Task.CompletedTask;
        }

        #region Convenience methods
        public static Task InfoAsync<T>(string message, Exception exception = null)
            where T : class => LogAsync(LogSeverity.Info, typeof(T).Name, message, exception);

        public static Task DebugAsync<T>(string message, Exception exception = null)
            where T : class => LogAsync(LogSeverity.Debug, typeof(T).Name, message, exception);

        public static Task ErrorAsync<T>(string message, Exception exception = null)
            where T : class => LogAsync(LogSeverity.Error, typeof(T).Name, message, exception);

        public static Task WarningAsync<T>(string message, Exception exception = null)
            where T : class => LogAsync(LogSeverity.Warning, typeof(T).Name, message, exception);

        public static Task VerboseAsync<T>(string message, Exception exception = null)
            where T : class => LogAsync(LogSeverity.Verbose, typeof(T).Name, message, exception);

        public static Task CriticalAsync<T>(string message, Exception exception = null)
            where T : class => LogAsync(LogSeverity.Critical, typeof(T).Name, message, exception);
        #endregion
    }
}
