using System;
using CommonBotLibrary.Services;
using Discord;

namespace Magnanibot.Extensions
{
    public static class EmbedBuilderExtensions
    {
        /// <summary> 
        ///   Adds a formatted description to the <see cref="Embed"/>.
        /// </summary>
        /// <param name="builder">This builder instance.</param>
        /// <param name="user">The user to address in this description.</param>
        /// <param name="action">The sentence fragment after the user's name.</param>
        /// <param name="emoji">The Unicode emoji to use before the sentence.</param>
        /// <param name="withComma">Whether or not to place a comma before the action.</param>
        public static EmbedBuilder WithUserAction(
            this EmbedBuilder builder, string emoji, IUser user, string action, bool withComma = false)
        {
            var userName = (user as IGuildUser)?.Nickname ?? user.Username;
            var comma = withComma ? "," : string.Empty;

            return builder.WithDescription($"{emoji} **{userName}**{comma} {action}");
        }

        public static EmbedBuilder WithRandomColor(this EmbedBuilder builder)
        {
            var colorHex = string.Format // get random 6-digit hex number
                ("{0:X6}", RandomService.Generator.Next(0x1000000));

            return builder.WithColor(new Color(Convert.ToUInt32(colorHex, 16)));
        }

        public static EmbedBuilder WithNullableImageUrl(
            this EmbedBuilder builder, string poster)
        {
            if (poster != null)
                builder.WithImageUrl(poster);
            return builder;
        }

        public static EmbedBuilder WithNullableThumbnailUrl(
            this EmbedBuilder builder, string poster)
        {
            if (poster != null)
                builder.WithThumbnailUrl(poster);
            return builder;
        }

        public static EmbedBuilder WithImageUrl(
            this EmbedBuilder builder, bool condition, string poster)
        {
            if (condition)
                builder.WithImageUrl(poster);
            return builder;
        }

        public static EmbedBuilder WithThumbnailUrl(
            this EmbedBuilder builder, bool condition, string poster)
        {
            if (condition)
                builder.WithThumbnailUrl(poster);
            return builder;
        }

        public static EmbedBuilder WithInlineField(
            this EmbedBuilder builder, string name, string value)
        {
            return builder.AddField(field =>
            {
                field.Name = name;
                field.Value = value;
                field.IsInline = true;
            });
        }

        public static EmbedBuilder WithAuthor(
            this EmbedBuilder builder, string name, string iconUrl)
        {
            return builder.WithAuthor(a =>
            {
                a.Name = name;
                a.IconUrl = iconUrl;
            });
        }

        public static EmbedBuilder WithFooter(
            this EmbedBuilder builder, string text)
        {
            return builder.WithFooter(f => f.Text = text);
        }
    }
}
