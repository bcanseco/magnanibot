using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;
using Magnanibot.Models;
using Magnanibot.Services;
using Difficulty = CommonBotLibrary.Services.Models.OpenTriviaDbDifficulty;
using Category = CommonBotLibrary.Services.Models.OpenTriviaDbCategory;
using Type = CommonBotLibrary.Services.Models.OpenTriviaDbType;

namespace Magnanibot.Modules
{
    [Group(nameof(Trivia))]
    [Summary("Starts an interactive trivia session.")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Trivia : Module
    {
        public Trivia(OpenTriviaDbService service, ReactionCoordinator coordinator)
            => (Service, Coordinator) = (service, coordinator);

        private OpenTriviaDbService Service { get; }
        private ReactionCoordinator Coordinator { get; }

        private static string[] OptionEmojis { get; } = {"🇦", "🇧", "🇨", "🇩"};
        private static IDictionary<string, int> DifficultySeconds { get; }
            = new Dictionary<string, int> {{"easy", 10}, {"medium", 20}, {"hard", 30}};

        [Command, Summary("Uses a random question with any difficulty and category.")]
        [Remarks("Example: !trivia")]
        private async Task GetAsync()
            => await GetWithParamsAsync();

        [Command, Summary("Uses a random question with a particular difficulty (easy/medium/hard).")]
        [Remarks("Example: !trivia hard")]
        private async Task GetAsync(Difficulty difficulty)
            => await GetWithParamsAsync(default(Category), difficulty);

        [Command, Summary("Uses a random question with a [particular category](https://pastebin.com/5xnQn0Qr).")]
        [Remarks("Example: !trivia film")]
        private async Task GetAsync([Remainder] Category category)
            => await GetWithParamsAsync(category);

        [Command, Summary("Uses a random question with a particular difficulty and category.")]
        [Remarks("Example: !trivia easy general knowledge")]
        private async Task GetAsync(Difficulty difficulty, [Remainder] Category category)
            => await GetWithParamsAsync(category, difficulty);

        private async Task GetWithParamsAsync(Category category = default(Category), Difficulty? difficulty = null)
        {
            var trivia = (await Service.GetTriviaAsync(1, category, difficulty, Type.Multiple)).First();
            var correctPosition = RandomService.Generator.Next(4);
            var secondsRemaining = DifficultySeconds[trivia.Difficulty];

            var builder = new EmbedBuilder()
                .WithTitle($"📚 {WebUtility.HtmlDecode(trivia.Question)}")
                .WithRandomColor()
                .WithDescription($"**Category:** {trivia.Category}\n" +
                                 $"**Difficulty:** {trivia.Difficulty}")
                .WithFooter("React with the emoji of the correct answer.");

            var incorrectAnswerIterator = trivia.IncorrectAnswers.GetEnumerator();
            incorrectAnswerIterator.MoveNext();

            var sb = new StringBuilder();
            foreach (var choiceNumber in Enumerable.Range(0, 4))
            {
                string choice;
                if (correctPosition == choiceNumber)
                {
                    choice = trivia.CorrectAnswer;
                }
                else
                {
                    choice = incorrectAnswerIterator.Current;
                    incorrectAnswerIterator.MoveNext();
                }

                sb.AppendLine($"{OptionEmojis[choiceNumber]} - {WebUtility.HtmlDecode(choice)}");
            }
            builder.AddField("Initializing...", sb.ToString());
            incorrectAnswerIterator.Dispose();
            
            var message = new TalliedMessage(secondsRemaining, builder, async (emojiUsers, sentMessage) =>
            {
                // This block is the callback that gets invoked by the message itself
                // 10-30 seconds after being sent (once the voting period is over).
                var correctChoice = emojiUsers.Keys.ElementAt(correctPosition);

                builder.Fields.First().Name =
                    $"Voting has ended. The correct answer was {correctChoice}.";
                builder.Fields.First().Value =
                    builder.Fields.First().Value.ToString().Replace(correctChoice, "✅");

                if (emojiUsers[correctChoice].Any())
                {
                    var winners = emojiUsers[correctChoice]
                        .Select(u => u.Nickname ?? u.Username)
                        .Aggregate((accumulator, piece) => $"{accumulator}, {piece}");

                    builder.WithFooter($"🏅 Winner(s): {winners}");
                }
                else
                {
                    builder.WithFooter("💩 Nobody got it right.");
                }

                await sentMessage.ModifyAsync(m => m.Embed = builder.Build());
                await sentMessage.RemoveAllReactionsAsync();
            });

            await Coordinator.SendInteractiveMessageAsync(Context, message);
        }
    }
}
