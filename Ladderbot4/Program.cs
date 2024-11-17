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
using Microsoft.Extensions.Hosting;
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
        private static SettingsManager _settingsManager;
        private static SettingsData _settingsData;

        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<DiscordSocketClient>();
                    services.AddSingleton<CommandService>();
                    services.AddSingleton<InteractionService>();

                    services.AddSingleton<ChallengeData>();
                    services.AddSingleton<HistoryData>();
                    services.AddSingleton<LadderData>();
                    services.AddSingleton<SettingsData>();
                    services.AddSingleton<TeamData>();

                    services.AddSingleton<HistoryManager>();
                    services.AddSingleton<ChallengeManager>();
                    services.AddSingleton<LadderManager>();
                    services.AddSingleton<MemberManager>();
                    services.AddSingleton<StatesManager>();
                    services.AddSingleton<SettingsManager>();
                    services.AddSingleton<TeamManager>();
                })
                .Build();

            // Retrieve the SettingsManager from the DI container
            _settingsManager = host.Services.GetRequiredService<SettingsManager>();
            _settingsData = host.Services.GetRequiredService<SettingsData>();

            // Ensure the settings are loaded correctly
            _settingsManager.SetBotTokenProcess();

            var program = new Program();
            program._services = host.Services;

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

            // Init interaction service for Slash Commands
            _interactionService = new InteractionService(_client.Rest);

            _commands.Log += Log;


            // Set up event handlers
            _client.Log += Log;
            _client.Ready += ClientReady;
            _client.InteractionCreated += HandleInteractionAsync;
            _client.MessageReceived += HandleCommandAsync;

            // Login and start bot
            // Token is pulled from contents of config.json using SettingsManager
            await _client.LoginAsync(TokenType.Bot, _settingsManager.Settings.DiscordBotToken);
            await _client.StartAsync();

            // Load Non-Slash Commands (For set_guild_id to setup SlashCommands)
            await _commands.AddModuleAsync<NonSlashCommands>(_services);
            Console.WriteLine("\t\tNon-SlashCommand modules added to CommandService");

            // Keep the bot running
            await Task.Delay(-1);
        }

        private async Task ClientReady()
        {
            // Register all SlashCommand modules
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Setup/Check GuildId Process
            if (!_settingsManager.IsGuildIdSet())
            {
                Console.WriteLine("\nInvalid Guild ID found. Please select your guild from the list below:");

                // List all guilds the bot is currently in
                foreach (var guild in _client.Guilds)
                {
                    Console.WriteLine($"\t- {guild.Name} (ID: {guild.Id})");
                }

                Console.WriteLine("\nPlease enter the Guild Id you would like to use (If entered incorrectly, use /set_guild_id or /sgid once connected to Discord then restart the bot and SlashCommands should begin to pop up. To change GuildId manually then ensure the bot is closed, edit and save the config.json file in the Settings folder, and restart the bot. SlashCommands should begin to show now as well once you are connected. Refer to the documentation for more information): ");
                string? guildId = Console.ReadLine();

                // Validate the input
                if (ulong.TryParse(guildId, out ulong selectedGuildId) && _client.GetGuild(selectedGuildId) != null)
                {
                    _settingsManager.Settings.GuildId = selectedGuildId;
                    _settingsManager.SaveSettings();
                    _settingsManager.LoadSettingsData();
                    Console.WriteLine($"Guild ID set to {selectedGuildId}.");
                }
                else
                {
                    Console.WriteLine("Invalid Guild ID entered. Please restart the bot and try again.");
                    return;
                }
            }

            // Register all slash commands to GuildId from setup process/config.json
            await _interactionService.RegisterCommandsToGuildAsync(_settingsManager.Settings.GuildId);
            Console.WriteLine("\tBot\t\tSlashCommand modules registered to Interaction Service.");
            Console.WriteLine($"\tBot\t\tGuild Id for SlashCommands set to: {_settingsManager.Settings.GuildId}");
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