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
    [Group(nameof(Imgur)), Alias("i", "images", "pictures", "photos")]
    [Summary("Searches Imgur for images.")]
    [Remarks("Example: !imgur pizza")]
    [RequireContext(ContextType.Guild)]
    public class Imgur : Module
    {
        private Imgur(ImgurService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private ImgurService Service { get; }
        private ReactionCoordinator Coordinator { get; }

        [Command]
        private async Task GetAsync([Remainder] string query)
        {
            var images = (await Service.SearchAsync(query))
                .Select(i => new EmbedBuilder()
                    .WithTitle(i.Title)
                    .WithDescription(i.Description?.Truncate(300))
                    .WithUrl($"https://imgur.com/{i.Id}")
                    .WithImageUrl(i.Url)
                    .WithColor(new Color(0x89c623)))
                .ToList();

            if (!images.Any())
                throw new BotException($"No images were found matching \"{query}\".");

            var message = new PaginatedMessage(images, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
    }
}
