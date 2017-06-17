using CommonBotLibrary;
using Newtonsoft.Json;

namespace Magnanibot
{
    public class BotTokens : Tokens
    {
        [JsonProperty]
        public static string Discord { get; set; }

        [JsonProperty]
        public static string MySql { get; set; }

        [JsonProperty]
        public static string Alias { get; set; } = nameof(Magnanibot);
    }
}
