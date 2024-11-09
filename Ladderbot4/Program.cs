using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Ladderbot4;
using Ladderbot4.Commands;
using Ladderbot4.Data;
using Ladderbot4.Managers;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    private DiscordSocketClient _client;
    private CommandService _commands;
    private IServiceProvider _services;

    public static async Task Main(string[] args)
    {
        var program = new Program();
        await program.RunBotAsync();
    }

    public async Task RunBotAsync()
    {
        // Initialize Discord client and command service
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Info,
            GatewayIntents = GatewayIntents.AllUnprivileged |
            GatewayIntents.MessageContent |
            GatewayIntents.GuildMessages |
            GatewayIntents.Guilds
        });

        _commands = new CommandService();
        _commands.Log += Log;
        _services = ConfigureServices();

        // Set up event handlers
        _client.Log += Log;
        _client.MessageReceived += HandleCommandAsync;

        // Login and start bot
        await _client.LoginAsync(TokenType.Bot, Token.discordToken);
        await _client.StartAsync();

        // Load commands
        await _commands.AddModuleAsync<TeamMemberCommands>(_services);
        Console.WriteLine("Command modules added to CommandService");

        // Keep the bot running
        await Task.Delay(-1);
    }

    private IServiceProvider ConfigureServices()
    {
        // Register Discord client, command service, and modules
        return new ServiceCollection()
            .AddSingleton(_client)

            // Add Read/Write Data Helpers
            .AddSingleton<TeamData>()

            // Add Managers
            .AddSingleton<LadderManager>()
            .AddSingleton<TeamManager>()
            .AddSingleton<MemberManager>()

            // All Commands are loaded into _commands in RunBotAsync
            .AddSingleton(_commands)

            .BuildServiceProvider();
    }

    private async Task HandleCommandAsync(SocketMessage socketMessage)
    {
        Console.WriteLine("Message received.");
        Console.WriteLine($"Message Content: {socketMessage.Content}"); // For Debugging

        // Ignore system messages and check if it's a user message
        if (socketMessage is not SocketUserMessage message || message.Author.IsBot)
        {
            return;
        }

        // Set up argument position for prefix parsing
        int argPos = 0;

        // Check if the message has the correct prefix ('!')
        if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
        {
            var context = new SocketCommandContext(_client, message);

            // Execute command
            var result = await _commands.ExecuteAsync(context, argPos, _services);

            // Log any command errors
            if (!result.IsSuccess)
            {
                Console.WriteLine($"Command Error: {result.ErrorReason}");
            }
        }
    }

    private Task Log(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }
}
