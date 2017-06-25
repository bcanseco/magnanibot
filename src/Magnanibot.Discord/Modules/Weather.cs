using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Weather)), Alias("w")]
    [Summary("Gets current weather for a location.")]
    [Remarks("Example: !weather miami")]
    public class Weather : Module
    {
        public Weather(OpenWeatherMapService service)
            => Service = service;

        private OpenWeatherMapService Service { get; }

        [Command]
        private async Task GetAsync([Remainder] string city)
        {
            var weather = await Service.GetCurrentWeatherAsync(city);
            
            await EmbedAsync(new EmbedBuilder()
                .WithColor(new Color(0x8bcbf6))
                .WithInlineField("🌎 Location", $"{weather.Name}, {weather.CountryInitials}")
                .WithInlineField("🌡 Temperature", $"{weather.Temperature}°F")
                .WithInlineField("☁ Conditions", $"{weather.Conditions}")
                .WithInlineField("😓 Humidity", $"{weather.Humidity}%")
                .WithInlineField("💨 Wind", $"{weather.WindSpeed} mph")
                .WithInlineField("📏 Lat, Long", $"{weather.Location.Latitude}, {weather.Location.Longitude}"));
        }
    }
}
