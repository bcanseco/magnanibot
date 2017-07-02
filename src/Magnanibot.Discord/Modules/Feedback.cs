using System;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Feedback)), Alias("idea", "suggestion", "bug")]
    [Summary("Creates a GitHub issue with your feedback. Thank you!")]
    [Remarks("Example: !feedback Add more commands plz")]
    [RequireContext(ContextType.Guild)]
    public class Feedback : Module
    {
        private Feedback(GithubService service)
            => Service = service;

        private GithubService Service { get; }

        [Command]
        private async Task PostAsync([Remainder] string comment)
        {
            var title = comment.Truncate(55);
            var body = $"**Server:** {Context.Guild.Name}" +
                       $"\n**Channel:** {Context.Channel.Name}" +
                       $"\n**User:** {Context.User}" +
                       $"\n\n**Message:**\n{comment}" +
                       $"\n\n`Filed on {DateTime.Now} using Discord.NET {DiscordConfig.Version}.`";
            var labels = new[] {"feedback"};

            var isSuccess = await Service.CreateIssueAsync(title, body, labels);
            // Positive feedback to be delivered via webhook
            if (!isSuccess) throw new BotException("Couldn't process feedback, please try again later.");
        }
    }
}
