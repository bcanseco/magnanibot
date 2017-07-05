using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Google.Apis.Vision.v1.Data;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;
using Color = Discord.Color;

namespace Magnanibot.Modules
{
    [Group(nameof(Analyze)), Alias("vision", "ana")]
    [Summary("Uses machine learning to analyze images.")]
    [RequireContext(ContextType.Guild)]
    public class Analyze : Module
    {
        public Analyze(GoogleVisionService service)
            => Service = service;

        private GoogleVisionService Service { get; }

        [Command, Summary("Analyzes an attached image.")]
        [Remarks("Example: !analyze")]
        private async Task GetAsync()
        {
            var url = Context.Message.Attachments.FirstOrDefault()?.Url
                      ?? throw new BotException("Either use a link or upload an image.");

            await GetAsync(new Uri(url));
        }

        [Command, Summary("Analyzes an image using an image URL.")]
        [Remarks("Example: !analyze https://i.imgur.com/g3D5jNz.jpg")]
        private async Task GetAsync([Remainder] Uri url)
        {
            var result = await Service.AnalyzeAsync(await url.AsBase64Url());
            var dominantColor = result.ImagePropertiesAnnotation.DominantColors.Colors.First().Color;

            var embed = new EmbedBuilder()
                .WithThumbnailUrl(url.AbsoluteUri)
                .WithFooter("Powered by Google Cloud Vision.")
                .WithColor(new Color(
                    (int) (dominantColor.Red ?? 0F),
                    (int) (dominantColor.Green ?? 0F),
                    (int) (dominantColor.Blue ?? 0F)));

            if (result.WebDetection != null)
            {
                var entities = result.WebDetection.WebEntities
                    .Where(e => !string.IsNullOrWhiteSpace(e.Description))
                    .Select(e => $"• {e.Description}");

                embed.WithInlineField("Entities", string.Join("\n", entities));
            }

            if (result.LabelAnnotations != null)
            {
                var labels = result.LabelAnnotations.Take(3).Select(l => $"• {l.Description} ({l.Score:0.00%})");
                embed.WithInlineField("Labels", string.Join("\n", labels));
            }

            if (result.FullTextAnnotation != null)
                embed.WithInlineField("Text", result.FullTextAnnotation.Text);

            if (result.FaceAnnotations != null && result.FaceAnnotations.Count == 1)
            {
                var face = result.FaceAnnotations[0];
                var likelyEmotions = typeof(FaceAnnotation).GetProperties()
                    .Where(p => p.Name.Contains("Likelihood"))
                    .Where(p => (string)p.GetValue(face) == "VERY_LIKELY")
                    .Select(p => $"• {p.Name.Replace("Likelihood", string.Empty)}")
                    .ToList();

                if (likelyEmotions.Any())
                    embed.WithInlineField("Likely emotions", string.Join("\n", likelyEmotions));
            }

            await EmbedAsync(embed);
        }
    }
}
