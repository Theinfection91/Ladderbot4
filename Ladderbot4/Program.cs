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

        private InteractionService _interactionService;

        private static SettingsManager _settingsManager;
        private static SettingsData _settingsData;

        public static async Task Main(string[] args)
        {
            var program = new Program();
            await program.RunAsync();
        }

        public async Task RunAsync()
        {
            // Create a shared instance of DiscordSocketClient
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents =
                    GatewayIntents.AllUnprivileged |
                    GatewayIntents.MessageContent |
                    GatewayIntents.GuildMessages |
                    GatewayIntents.Guilds
            });

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Register the shared DiscordSocketClient instance
                    services.AddSingleton(_client);

                    // Register other services
                    services.AddSingleton<CommandService>();
                    services.AddSingleton<InteractionService>();

                    services.AddSingleton<ChallengeData>();                   
                    services.AddSingleton<HistoryData>();
                    services.AddSingleton<LadderData>();
                    services.AddSingleton<LeagueData>();
                    services.AddSingleton<MemberData>();
                    services.AddSingleton<SettingsData>();

                    services.AddSingleton<AchievementManager>();
                    services.AddSingleton<ChallengeManager>();
                    services.AddSingleton<EmbedManager>();
                    services.AddSingleton<GitBackupManager>();
                    services.AddSingleton<HistoryManager>();
                    services.AddSingleton<LadderManager>();
                    services.AddSingleton<LeagueManager>();
                    services.AddSingleton<MemberManager>();
                    services.AddSingleton<StatesManager>();
                    services.AddSingleton<SettingsManager>();
                    services.AddSingleton<TeamManager>();
                })
                .Build();

            // Retrieve required services
            _services = host.Services;
            _settingsManager = _services.GetRequiredService<SettingsManager>();
            _settingsData = _services.GetRequiredService<SettingsData>();

            // Ensure settings are loaded
            _settingsManager.SetBotTokenProcess();

            // TODO - Implement working Git PAT Token and URL Process
            _settingsManager.SetGitBackupProcess();

            await RunBotAsync();
        }

        public async Task RunBotAsync()
        {
            // Initialize CommandService and InteractionService
            _commands = _services.GetRequiredService<CommandService>();
            _interactionService = new InteractionService(_client.Rest);

            _commands.Log += Log;

            // Set up event handlers
            _client.Log += Log;
            _client.Ready += ClientReady;
            _client.InteractionCreated += HandleInteractionAsync;
            _client.MessageReceived += HandleCommandAsync;

            // Add the Disconnected event handler to automatically reconnect
            _client.Disconnected += async (exception) =>
            {
                Console.WriteLine("Disconnected from Discord. Reconnecting...");
                await Task.Delay(5000); // Wait before reconnecting
                await _client.StartAsync(); // Reconnect
            };

            // Login and start the bot
            await _client.LoginAsync(TokenType.Bot, _settingsManager.Settings.DiscordBotToken);
            await _client.StartAsync();

            // Wait for Ready event
            var readyTask = new TaskCompletionSource<bool>();
            _client.Ready += () =>
            {
                readyTask.SetResult(true);
                return Task.CompletedTask;
            };

            Console.WriteLine("Waiting for bot to be ready...");
            await readyTask.Task;

            // Load Non-Slash Commands
            await _commands.AddModuleAsync<NonSlashCommands>(_services);
            Console.WriteLine("\t\tNon-SlashCommand modules added to CommandService");

            Console.WriteLine($"Bot logged in as: {_client.CurrentUser?.Username ?? "null"}");

            // Keep the bot running
            await Task.Delay(-1);
        }

        private async Task ClientReady()
        {
            // Register SlashCommand modules
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Setup guild ID if not set
            _settingsManager.SetGuildIdProcess();

            // Register commands to guild
            await _interactionService.RegisterCommandsToGuildAsync(_settingsManager.Settings.GuildId);
            Console.WriteLine($"Commands registered to guild {_settingsManager.Settings.GuildId}");
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(context, _services);
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            if (socketMessage is not SocketUserMessage message || message.Author.IsBot) return;

            int argPos = 0;
            if (message.HasStringPrefix(_settingsManager.Settings.CommandPrefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                    Console.WriteLine($"Command Error: {result.ErrorReason}");
            }
        }

        private Task Log(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}