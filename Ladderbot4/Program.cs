using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Commands;
using Ladderbot4.Data;
using Ladderbot4.Managers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;

namespace Ladderbot4
{
    public class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        // For Slash Commands
        private InteractionService _interactionService;

        // To grab bot token and command prefix
        private SettingsManager _settingsManager;
        private SettingsData _settingsData;

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
                GatewayIntents =
                    GatewayIntents.AllUnprivileged |
                    GatewayIntents.MessageContent |
                    GatewayIntents.GuildMessages |
                    GatewayIntents.Guilds
            });

            // Init CommandService
            _commands = new CommandService();

            // Init SettingsData and SettingsManager for config.json bot data
            _settingsData = new SettingsData();
            _settingsManager = new SettingsManager(_settingsData);

            // Init interaction service for Slash Commands
            _interactionService = new InteractionService(_client.Rest);

            _commands.Log += Log;
            _services = ConfigureServices();

            // Set up event handlers
            _client.Log += Log;
            _client.Ready += ClientReady;
            _client.InteractionCreated += HandleInteractionAsync;
            _client.MessageReceived += HandleCommandAsync;

            // Login and start bot
            // Token is pulled from contents of config.json using SettingsManager
            await _client.LoginAsync(TokenType.Bot, _settingsManager.Settings.DiscordBotToken);
            await _client.StartAsync();

            // Load commands
            await _commands.AddModuleAsync<ChallengeCommands>(_services);
            await _commands.AddModuleAsync<SettingsTestingCommands>(_services);
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
                
                // Add Interaction Service
                .AddSingleton(_interactionService)

                // Add Read/Write Data Helpers
                .AddSingleton<ChallengeData>()
                .AddSingleton(_settingsData)
                .AddSingleton<TeamData>()

                // Add Managers
                .AddSingleton<ChallengeManager>()
                .AddSingleton<LadderManager>()
                .AddSingleton<MemberManager>()
                .AddSingleton(_settingsManager)
                .AddSingleton<TeamManager>()
            
                // All Commands are loaded into _commands in RunBotAsync
                .AddSingleton(_commands)

                .BuildServiceProvider();
        }

        private async Task ClientReady()
        {
            // Register all slash commands
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _interactionService.RegisterCommandsToGuildAsync(_settingsManager.Settings.GuildId);
            Console.WriteLine("Slash commands registered");
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(context, _services);
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
            if (message.HasStringPrefix(_settingsManager.Settings.CommandPrefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
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
}