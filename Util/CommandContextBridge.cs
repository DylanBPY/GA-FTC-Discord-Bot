using Discord;
using Discord.Commands;
using Discord.Interactions;

namespace Util;

public interface ContextBridge
{
    IUser User { get; }
    IGuild? Guild { get; }
    Task DeferAsync();
    Task ReplyAsync(string? message = null, Embed? embed = null);
}

public class PrefixContextBridge : ContextBridge
{
    private readonly SocketCommandContext _ctx;
    public PrefixContextBridge(SocketCommandContext ctx) => _ctx = ctx;

    public IUser User => _ctx.User;
    public IGuild? Guild => _ctx.Guild;

    public Task DeferAsync() => Task.CompletedTask;
    public Task ReplyAsync(string? message = null, Embed? embed = null) {
        return _ctx.Channel.SendMessageAsync(text: message, embed: embed);
    }
}

public class SlashContextBridge : ContextBridge
{
    private readonly SocketInteractionContext _ctx;
    public SlashContextBridge(SocketInteractionContext ctx) => _ctx = ctx;

    public IUser User => _ctx.User;
    public IGuild? Guild => _ctx.Guild;

    public async Task DeferAsync() => await _ctx.Interaction.DeferAsync();
    public async Task ReplyAsync(string? message = null, Embed? embed = null)
    {
        if (_ctx.Interaction.HasResponded)
        {
            await _ctx.Interaction.FollowupAsync(text: message, embed: embed);
        }
        else
        {
            await _ctx.Interaction.RespondAsync(text: message, embed: embed);
        }
    }
}