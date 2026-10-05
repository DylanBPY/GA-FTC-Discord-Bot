using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace Util;

public class Logger
{
    public Logger(DiscordSocketClient client, CommandService commands, InteractionService interactions)
    {
        client.Log += Log;
        commands.Log += Log;
        interactions.Log += Log;
    }

    public static Task Log(LogMessage message)
    {
        var color = message.Severity switch
        {
            LogSeverity.Critical or LogSeverity.Error => ConsoleColor.Red,
            LogSeverity.Warning => ConsoleColor.Yellow,
            LogSeverity.Info => ConsoleColor.Green,
            LogSeverity.Verbose or LogSeverity.Debug => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        Console.ForegroundColor = color;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{message.Severity}] [{message.Source}] {message.Message} {message.Exception}");
        Console.ResetColor();

        return Task.CompletedTask;
    }
}