using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Magnanibot.Context;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;
using Microsoft.EntityFrameworkCore;
using TrophyModel = Magnanibot.Context.Models.Trophy;

namespace Magnanibot.Modules
{
    [Group(nameof(Trophy)), Alias("trophies", "awards", "award")]
    [Summary("View trophies for each user.")]
    [RequireContext(ContextType.Guild)]
    public class Trophy : Module
    {
        [Command, Summary("Displays the trophy leaderboard.")]
        [Remarks("Example: !trophy")]
        private async Task GetAsync()
        {
            using (var context = new BotContext())
            {
                var leaderboard = await context.Trophies
                    .AsNoTracking()
                    .GroupBy(t => t.AwardedTo)
                    .Select(group => new
                    {
                        Awardee = group.Key,
                        TrophyCount = group.Count()
                    })
                    .OrderByDescending(group => group.TrophyCount)
                    .Select(group => $"🔸 **{group.Awardee}:** {group.TrophyCount}")
                    .ToListAsync();

                var totalTrophies = await context.Trophies.CountAsync();

                await EmbedAsync(new EmbedBuilder()
                    .WithTitle("🏆 Trophy leaderboard")
                    .WithColor(new Color(0xc1684f))
                    .WithFooter($"Total awarded: {totalTrophies} trophies.")
                    .WithDescription("Use `!trophy [user]` to view a user's trophies.\n" +
                                     $"\n{string.Join("\n", leaderboard)}"));
            }
        }

        [Command, Summary("Displays a user's trophies.")]
        [Remarks("Example: !trophy @Fred#0001")]
        private async Task GetByIdAsync([Remainder] IUser user)
        {
            using (var context = new BotContext())
            {
                var trophies = await context.Trophies
                    .AsNoTracking()
                    .Where(t => t.AwardedTo == user.Username)
                    .OrderByDescending(t => t.AwardedOn)
                    .ToListAsync();

                var userName = (user as IGuildUser)?.Nickname ?? user.Username;

                var embed =  new EmbedBuilder()
                    .WithTitle($"🏆 **{userName}**'s trophies")
                    .WithDescription("Use `!trophy` to see the overall leaderboard.")
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .WithColor(new Color(0xffac33));

                foreach (var trophy in trophies)
                {
                    var title = $"From **{trophy.AwardedBy}** on {trophy.AwardedOn:MM/dd/yyyy}:";
                    embed.AddField(title, trophy.Reason);
                }

                if (!trophies.Any())
                    embed.WithFooter("This user doesn't have any trophies.");

                await EmbedAsync(embed);
            }
        }

        [Command, Summary("Awards a new trophy to a user.")]
        [Remarks("Example: !trophy @Fred#0001 For being cool")]
        [RequireUserPermission(GuildPermission.Administrator)]
        private async Task PostAsync(IUser user, [Remainder] string reason)
        {
            if (user.Equals(Context.User))
                throw new BotException("You cannot award yourself a trophy.");

            using (var context = new BotContext())
            {
                var awardedBy = Context.User.Username;
                var awardedTo = user.Username;
                var trophy = new TrophyModel(awardedBy, awardedTo, reason);

                var addedTrophy = await context.Trophies.AddAsync(trophy);
                await context.SaveChangesAsync();

                await EmbedAsync(new EmbedBuilder()
                    .WithTitle($"🏆 Trophy #{addedTrophy.Entity.Id} awarded")
                    .WithColor(new Color(0xffcc4d))
                    .WithDescription($"**{awardedBy}** has given **{awardedTo}** a trophy!")
                    .AddField("Reason:", reason));

                await Context.Message.DeleteAsync();
            }
        }
    }
}
