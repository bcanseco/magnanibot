using System.ComponentModel.DataAnnotations;
using Discord;
using Magnanibot.Extensions;

namespace Magnanibot.Context.Models
{
    public class Memory
    {
        public Memory() { /* required by EF */ }
        public Memory(string text)
            => Text = text;

        [Key]
        public int Id { get; set; }
        public string Text { get; set; }

        public override string ToString() => Text;

        public static implicit operator EmbedBuilder(Memory m)
            => new EmbedBuilder()
                .WithTitle($"Memory #{m.Id}")
                .WithDescription(m.Text)
                .WithRandomColor();
    }
}
