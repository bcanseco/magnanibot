using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Magnanibot.Context;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;
using Magnanibot.Models;
using Magnanibot.Services;
using Microsoft.EntityFrameworkCore;

namespace Magnanibot.Modules
{
    [Group(nameof(Memory)), Alias("meme", "m")]
    [Summary("The only command you care about.")]
    [RequireBotPermission(ChannelPermission.ManageMessages)]
    [RequireContext(ContextType.Guild)]
    public class Memory : Module
    {
        public Memory(ReactionCoordinator coordinator)
            => Coordinator = coordinator;

        private ReactionCoordinator Coordinator { get; }

        [Command, Summary("Gets a random assortment of memories from the database.")]
        [Remarks("Example: !memory")]
        private async Task GetRandomAsync()
        {
            using (var context = new BotContext())
            {
                var memories = await context.Memories
                    .OrderBy(m => Guid.NewGuid())
                    .Take(10)
                    .Select(m => (EmbedBuilder) m)
                    .AsNoTracking()
                    .ToListAsync();

                var message = new PaginatedMessage(memories, Context.User);
                await Coordinator.SendInteractiveMessageAsync(Context, message);
            }
        }

        [Command, Summary("Searches for memories from the database that contain text.")]
        [Remarks("Example: !memory kenny")]
        [Priority(1)]
        private async Task GetByTextAsync([Remainder] string text)
        {
            using (var context = new BotContext())
            {
                var memories = await context.Memories
                    .Where(m => m.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Take(20)
                    .Select(m => (EmbedBuilder) m)
                    .AsNoTracking()
                    .ToListAsync();

                if (!memories.Any())
                    throw new BotException($"No memories were found matching \"{text}\".");

                var message = new PaginatedMessage(memories, Context.User);
                await Coordinator.SendInteractiveMessageAsync(Context, message);
            }
        }

        [Command, Summary("Gets a specific memory from the database by ID.")]
        [Remarks("Example: !memory 19")]
        [Priority(2)]
        private async Task GetByIdAsync(int id)
        {
            using (var context = new BotContext())
            {
                var memory = await context.Memories
                    .AsNoTracking()
                    .SingleOrDefaultAsync(m => m.Id == id);

                if (memory == null)
                    throw new BotException($"No memory with ID: {id} was found.");

                await EmbedAsync(memory);
            }
        }

        [Command("add"), Alias("archive")]
        [Summary("Adds a new memory to the database.")]
        [Remarks("Example: !memory add That one time we did that one thing")]
        [Priority(3)]
        private async Task PostAsync([Remainder] string text)
        {
            using (var context = new BotContext())
            {
                await context.Memories.AddAsync(new Context.Models.Memory(text));
                await context.SaveChangesAsync();

                await EmbedAsync(new EmbedBuilder()
                    .WithUserAction("✍", Context.User, "successfully archived a memory.")
                    .WithColor(new Color(0x3f8bbf)));
            }
        }
    }
}
