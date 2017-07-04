using System.Threading.Tasks;
using CommonBotLibrary.Services;
using CommonBotLibrary.Services.Models;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Translate)), Alias("trans")]
    [Summary("Translates text to another language.")]
    public class Translate : Module
    {
        public Translate(YandexTranslateService service)
            => Service = service;

        private YandexTranslateService Service { get; }

        [Command, Summary("Translates with an explicitly provided source text language.")]
        [Remarks("Example: !translate english spanish How are you?")]
        [Priority(2)]
        private async Task GetAsync(YandexLanguage source, YandexLanguage target, [Remainder] string text)
            => await GetAsync(text, source, target);

        [Command, Summary("Translates by auto-detecting the source text's language.")]
        [Remarks("Example: !translate spanish How are you?")]
        [Priority(1)]
        private async Task GetAsync(YandexLanguage target, [Remainder] string text)
            => await GetAsync(text, null, target);

        private async Task GetAsync(string text, YandexLanguage source, YandexLanguage target)
        {
            var translation = await Service.TranslateAsync(text, target, source);

            await EmbedAsync(new EmbedBuilder()
                .WithColor(new Color(0x2faf8f))
                .WithTitle($"{translation.TargetLanguage} translation")
                .WithDescription(translation.OutputText)
                .WithFooter(translation.IsSourceDetected
                    ? $"Source language was detected as {translation.SourceLanguage}."
                    : $"Translated from {translation.SourceLanguage}."));
        }
    }
}
