using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Stats)), Alias("status", "ram", "latency", "uptime", "version")]
    [Summary("Shows bot statistics.")]
    [Remarks("Example: !stats")]
    public class Stats : Module
    {
        [Command]
        private async Task GetAsync()
        {
            var application = await Context.Client.GetApplicationInfoAsync();

            using (var process = Process.GetCurrentProcess())
            {
                await EmbedAsync(new EmbedBuilder()
                    .WithAuthor(BotTokens.Alias, Context.Client.CurrentUser.GetAvatarUrl())
                    .WithColor(new Color(0xFFFFFF))
                    .WithDescription($"Use `!profile `{Context.Client.CurrentUser.Mention} for more info.")
                    .WithInlineField("Owner", $"{application.Owner}")
                    .WithInlineField("Discord.NET version", DiscordConfig.Version)
                    .WithInlineField("Uptime", (DateTime.Now - process.StartTime).ToString(@"dd'd 'hh'h 'mm'm 'ss's'"))
                    .WithInlineField("Memory usage", $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}mb")
                    .WithInlineField("Latency", $"{(Context.Client as DiscordSocketClient)?.Latency}ms")
                    .WithInlineField("Threads", $"{process.Threads.Count}")
                    .WithFooter($"Hosted on {Environment.MachineName} ({RuntimeInformation.OSDescription.Truncate(35)})"));
            }
        }
    }
}
