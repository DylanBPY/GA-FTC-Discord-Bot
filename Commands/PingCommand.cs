using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot.Commands;

public static class Ping
{
    public static async Task Execute(Util.ContextBridge ctx, DiscordSocketClient client)
    {
        await ctx.ReplyAsync($"Pong (in {client.Latency}ms)");
    }
}

public class PingPrefixModule : ModuleBase<SocketCommandContext>
{
    private readonly DiscordSocketClient _client;
    public PingPrefixModule(DiscordSocketClient client) => _client = client;

    [Command("ping")]
    public Task Run() => Ping.Execute(new Util.PrefixContextBridge(Context), _client);
}

public class PingSlashModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly DiscordSocketClient _client;
    public PingSlashModule(DiscordSocketClient client) => _client = client;

    [SlashCommand("ping", "Check the latency of the bot")]
    public Task Run() => Ping.Execute(new Util.SlashContextBridge(Context), _client);
}