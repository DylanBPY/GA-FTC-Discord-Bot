using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Util;

class Bot
{
    public static int Season {get; } = 2026;

    private DiscordSocketClient? client;
    private ServiceProvider? services;

    public static Task Main(string[] args) => new Bot().MainAsync();

    public async Task MainAsync()
    {
        EnvLoader.Load();

        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers
        };

        client = new DiscordSocketClient(config);
        services = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton<CommandService>()
            .AddSingleton<InteractionService>(_ => new InteractionService(client))
            .AddSingleton<Logger>()
            .AddSingleton<Database>()
            .AddSingleton<Api>()
            .AddSingleton<Welcomer>()
            .BuildServiceProvider();

        services.GetRequiredService<Logger>();
        services.GetRequiredService<Database>();
        services.GetRequiredService<Api>();
        services.GetRequiredService<Welcomer>();

        CommandService prefixCommands = services.GetRequiredService<CommandService>();
        InteractionService slashCommands = services.GetRequiredService<InteractionService>();

        client.Ready += async () =>
        {
            // Register commands in the Commands package
            await prefixCommands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            await slashCommands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            await slashCommands.RegisterCommandsGloballyAsync();
        };

        client.MessageReceived += async msg =>
        {
            if (msg is not SocketUserMessage userMsg || userMsg.Author.IsBot) return;
            int argPos = 0;
            if (userMsg.HasCharPrefix('!', ref argPos))
            {
                var context = new SocketCommandContext(client, userMsg);
                await prefixCommands.ExecuteAsync(context, argPos, services);
            }
        };

        client.InteractionCreated += async interaction =>
        {
            var context = new SocketInteractionContext(client, interaction);
            await slashCommands.ExecuteCommandAsync(context, services);
        };

        string token = Environment.GetEnvironmentVariable("BOT_TOKEN") ?? string.Empty;
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("Environment variable for BOT_TOKEN must be set.");
        }
            
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Logger.Log(new LogMessage(LogSeverity.Info, "Bot", "Bot has started."));

        await Task.Delay(-1); // Run forever
    }
}