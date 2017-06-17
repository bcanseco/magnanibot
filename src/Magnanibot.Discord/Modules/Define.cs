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
    [Group(nameof(Define)), Alias("definition", "d")]
    [Summary("Gives the UrbanDictionary definition of a phrase.")]
    [Remarks("Example: !define magnanimous")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Define : Module
    {
        private Define(UrbanDictionaryService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private UrbanDictionaryService Service { get; }
        private ReactionCoordinator Coordinator { get; }

        [Command]
        private async Task GetAsync([Remainder] string phrase)
        {
            var definitions = (await Service.GetDefinitionsAsync(phrase))
                .Take(20)
                .Select(d => new EmbedBuilder()
                    .WithTitle(d.Word)
                    .WithUrl(d.Permalink)
                    .WithDescription(d.Definition.Truncate(360))
                    .AddField("Example", d.Example)
                    .WithColor(new Color(0x87A96B)))
                .ToList();

            if (!definitions.Any())
                throw new BotException($"No definitions were found for \"{phrase}\".");

            var message = new PaginatedMessage(definitions, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
    }
}
