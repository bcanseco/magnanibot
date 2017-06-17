using System.Threading.Tasks;
using CommonBotLibrary.Services;
using Discord;
using Discord.Commands;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Calculate)), Alias("calc", "c", "math", "evaluate", "eval")]
    [Summary("Evaluates a mathematical expression.")]
    [Remarks("Example: !calculate 2 + 2")]
    public class Calculate : Module
    {
        public Calculate(NCalcService service)
            => Service = service;

        private NCalcService Service { get; }

        [Command]
        private async Task GetAsync([Remainder] string expression)
        {
            var result = await Service.EvaluateAsync(expression);

            await EmbedAsync(new EmbedBuilder()
                .WithColor(new Color(0xf18b30))
                .WithUserAction("🤔", Context.User, $"your answer is: `{result}`", true));
        }
    }
}
