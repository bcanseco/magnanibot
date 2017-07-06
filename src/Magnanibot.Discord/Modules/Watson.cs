using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using CommonBotLibrary.Services.Models;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;
using Magnanibot.Models;
using Magnanibot.Services;

namespace Magnanibot.Modules
{
    [Group(nameof(Watson)), Alias("personality")]
    [Summary("Uses machine learning to analyze users.")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Watson : Module
    {
        public Watson(WatsonPersonalityService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private WatsonPersonalityService Service { get; }
        private ReactionCoordinator Coordinator { get; }

        [Command, Summary("Analyzes your own messages in the current channel.")]
        [Remarks("Example: !watson")]
        private async Task GetAsync()
            => await GetAsync(Context.User, Context.Channel as ITextChannel);

        [Command, Summary("Analyzes a user's messages in the current channel.")]
        [Remarks("Example: !watson @Wendy#1969")]
        private async Task GetAsync(IUser user)
            => await GetAsync(user, Context.Channel as ITextChannel);

        [Command, Summary("Analyzes a user's messages in a specific channel.")]
        [Remarks("Example: !watson @Wendy#1969 #general")]
        private async Task GetAsync(IUser user, ITextChannel channel)
        {
            IEnumerable<string> userMessages = new List<string>();

            await (await Context.Guild.GetChannelsAsync())
                .OfType<IMessageChannel>()
                .FirstOrDefault(ch => ch.Equals(channel))
                .GetMessagesAsync(1000)
                .ForEachAsync(batch =>
                {
                    userMessages = userMessages.Concat(batch
                        .Where(m => m.Author.Equals(user))
                        .Where(m => !string.IsNullOrWhiteSpace(m.Content))
                        .Where(m => !m.Content.StartsWith("!"))
                        .Select(m => m.Content));
                });

            var result = await Service.AnalyzeAsync(string.Join("\n", userMessages));

            // Preprocess response for first page of embed (Personality)
            var trees = typeof(WatsonPersonalityResult).GetProperties()
                .Where(prop => prop.PropertyType == typeof(List<WatsonPersonalityResult.TraitTree>))
                .Select(prop =>
                {
                    var list = (List<WatsonPersonalityResult.TraitTree>) prop.GetValue(result);
                    return (Name: prop.Name, Nodes: list.First().Children != null
                        ? list.SelectMany(trait => trait.Children)
                        : list);
                })
                .Select(tree => (Category: tree.Name, Traits: tree.Nodes
                    .GroupBy(trait => trait.Percentile >= 0.50)
                    .SelectMany(g => g.Key
                        ? g.OrderByDescending(t => t.Percentile).Take(3)
                        : g.OrderBy(t => t.Percentile).Take(3))
                    .OrderByDescending(t => t.Percentile)
                    .Select(t => $"{(t.Percentile >= .50 ? "📈" : "📉")} {t.Percentile:0%} - {t.Name}")));

            var firstPage = new EmbedBuilder()
                .WithAuthor($"{(user as IGuildUser)?.Nickname ?? user.Username}'s personality analysis",
                    user.GetAvatarUrl())
                .WithDescription($"Based on **{result.WordCount} words** (optimal: 3k+) in `#{channel}`.")
                .WithThumbnailUrl("https://i.imgur.com/AkrPIRN.png")
                .WithColor(new Color(0x5aa8f8));

            foreach (var pair in trees)
            {
                firstPage.WithInlineField(pair.Category, string.Join("\n", pair.Traits));
            }

            // Preprocess response for second page of embed (Consumption preferences)
            var consumptionPrefs = result.ConsumptionPreferences
                .Where(category => category.Name != "Purchasing Preferences" && category.Name != "Reading Preferences")
                .Select(category => (Category: category.Name, Preferences: category.ConsumptionPreferences
                    .Where(p => !p.Score.Equals(0.5))
                    .GroupBy(p => p.Score)
                    .SelectMany(g => g.Take(3))
                    .OrderByDescending(p => p.Score)
                    .Select(p => p.Name = p.Score.Equals(0)
                        ? p.Name.Replace("Likely", "❌ Not likely")
                        : $"👌 {p.Name}")
                    .ToList()))
                .ToList();

            var secondPage = new EmbedBuilder()
                .WithAuthor($"{(user as IGuildUser)?.Nickname ?? user.Username}'s consumption preferences",
                    user.GetAvatarUrl())
                .WithDescription($"Based on **{result.WordCount} words** (optimal: 3k+) in `#{channel}`.")
                .WithThumbnailUrl("https://i.imgur.com/AkrPIRN.png")
                .WithColor(new Color(0x4178bc));

            foreach (var pair in consumptionPrefs)
            {
                secondPage.AddField(pair.Category, string.Join("\n", pair.Preferences));
            }

            var message = new PaginatedMessage(new List<EmbedBuilder> {firstPage, secondPage}, Context.User);
            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
    }
}
