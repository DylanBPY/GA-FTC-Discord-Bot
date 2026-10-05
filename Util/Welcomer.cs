using Discord;
using Discord.WebSocket;

namespace Util;

public class Welcomer
{
    public Welcomer(DiscordSocketClient client)
    {
        client.UserJoined += OnUserJoinedAsync;
    }

    private async Task OnUserJoinedAsync(SocketGuildUser user)
    {
        var embed = new EmbedBuilder()
            .WithTitle($"Welcome to {user.Guild.Name}!")
            .WithDescription($"Welcome {user.Mention}! Glad to have you here.")
            .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
            .WithColor(Color.Orange)
            .WithCurrentTimestamp()
            .Build();

        await user.Guild.SystemChannel.SendMessageAsync(embed: embed);
    }
}