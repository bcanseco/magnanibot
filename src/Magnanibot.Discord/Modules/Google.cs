using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;
using Magnanibot.Models;
using Magnanibot.Services;

namespace Magnanibot.Modules
{
    [Group(nameof(Google)), Alias("g", "search", "bing")]
    [Summary("Gets Google search results.")]
    [Remarks("Example: !google cake recipe")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Google : Module
    {
        private Google(GoogleService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private GoogleService Service { get; }
        private ReactionCoordinator Coordinator { get; }
        
        [Command]
        private async Task GetAsync([Remainder] string query)
        {
            var results = (await Service.SearchAsync(query)).ToList();
            if (!results.Any()) throw new BotException($"No results were found for \"{query}\".");

            var embeds = new List<EmbedBuilder>();
            foreach (var chunk in results.Take(30).ChunksOf(3))
            {
                var embed = new EmbedBuilder()
                    .WithTitle($"\"{query}\"")
                    .WithUrl($"https://www.google.com/search?q={Uri.EscapeDataString(query)}")
                    .WithThumbnailUrl("https://i.imgur.com/4oCNuJg.png") // "G" logo
                    .WithDescription("A snippet of Google search results")
                    .WithColor(new Color(0x3fa85b));

                foreach (var result in chunk)
                    embed.AddField(result.Title, $"🔎 {result.Link}");

                embeds.Add(embed);
            }

            var message = new PaginatedMessage(embeds, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
    }
}
