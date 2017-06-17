using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Magnanibot.Exceptions;
using Magnanibot.Extensions;

namespace Magnanibot.Modules
{
    [Group(nameof(Avatar)), Alias("ava")]
    [Summary("Changes the bot's avatar.")]
    [RequireContext(ContextType.Guild)]
    public class Avatar : Module
    {
        [Command, Summary("Changes the bot's avatar using an image URL.")]
        [Remarks("Example: !avatar https://i.imgur.com/g3D5jNz.jpg")]
        private async Task PostAsync([Remainder] Uri url)
        {
            var base64 = await url.AsBase64Url();
            
            await Context.Client.CurrentUser.ModifyAsync(u =>
                u.Avatar = new Image(new MemoryStream(Convert.FromBase64String(base64))));
        }
        
        [Command, Summary("Changes the bot's avatar if an image is attached.")]
        [Remarks("Example: !avatar")]
        private async Task PostAsync()
        {
            var url = Context.Message.Attachments.FirstOrDefault()?.Url;

            if (url != null) await PostAsync(new Uri(url));
            else throw new BotException("Either use a link or upload an image.");
        }
    }
}
