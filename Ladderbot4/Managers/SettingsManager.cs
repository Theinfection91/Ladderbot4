using Discord.WebSocket;
using Ladderbot4.Data;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class SettingsManager
    {
        private DiscordSocketClient _client;
        private readonly SettingsData _settingsData;
        public Settings Settings { get; set; }

        public SettingsManager(DiscordSocketClient client, SettingsData settingsData)
        {
            _client = client;
            _settingsData = settingsData;
            Settings = _settingsData.LoadSettings();
        }

        public void LoadSettingsData()
        {
            Settings = _settingsData.LoadSettings();
        }

        public void SaveSettings()
        {
            _settingsData.SaveSettings(Settings);
        }

        public void SaveAndReloadSettingsDatabase()
        {
            SaveSettings();
            LoadSettingsData();
        }

        public bool IsDiscordIdInSuperAdminList(ulong discordId)
        {
            LoadSettingsData();

            foreach (ulong adminIds in Settings.SuperAdminDiscordIds)
            {
                if (adminIds == discordId) return true;
            }
            return false;
        }

        public bool IsGuildIdSet()
        {
            return Settings.GuildId != 0 && IsGuildIdValid();
        }

        public bool IsGuildIdValid()
        {
            return Settings.GuildId >= 15;
        }

        public bool IsGuildIdValidBool(ulong guildId)
        {
            return guildId >= 15;
        }

        public bool IsUserSuperAdmin(ulong userId)
        {
            foreach (ulong admin in Settings.SuperAdminDiscordIds)
            {
                if (admin == userId && Settings.SuperAdminMode) return true;
            }
            return false;
        }

        public bool IsValidBotTokenSet()
        {
            return !string.IsNullOrEmpty(Settings.DiscordBotToken) && Settings.DiscordBotToken != "ENTER_BOT_TOKEN_HERE" && IsValidBotToken(Settings.DiscordBotToken);
        }

        public bool IsValidBotToken(string botToken)
        {
            return botToken.Length >= 59;
        }

        public bool IsGitPatTokenSet()
        {
            return Settings.GitPatToken != "ENTER_GIT_PAT_TOKEN_HERE";
        }

        public void SetBotTokenProcess()
        {
            bool IsBotTokenProcessComplete = false;
            while (!IsBotTokenProcessComplete)
            {
                if (!IsValidBotTokenSet())
                {
                    Console.WriteLine("\nIncorrect Bot Token found in Settings\\config.json");
                    Console.WriteLine("Please enter your Bot Token now (This can be changed manually in Settings\\config.json as well if entered incorrectly and a connection can not be established): ");
                    string? botToken = Console.ReadLine();
                    if (IsValidBotToken(botToken))
                    {
                        Settings.DiscordBotToken = botToken;
                        SaveAndReloadSettingsDatabase();
                        IsBotTokenProcessComplete = true;
                    }
                    else
                    {
                        IsBotTokenProcessComplete = false;
                    }
                }
                else
                {
                    IsBotTokenProcessComplete= true;
                }
            }
        }

        public void SetGuildIdProcess()
        {
            bool IsGuildIdProcessComplete = false;
            while (!IsGuildIdProcessComplete)
            {                
                if (!IsGuildIdSet())
                {
                    Console.WriteLine("Incorrect Guild Id found in Settings\\config.json");
                    Console.WriteLine("Please set a valid Guild ID for SlashCommands.");
                    Console.WriteLine("Select a guild from the list below: ");
                    foreach (var guild in _client.Guilds)
                    {
                        Console.WriteLine($"Guild: {guild.Name} (ID: {guild.Id})");
                    }
                    string guildIdString = Console.ReadLine();
                    if (guildIdString != null)
                    {
                        if (ulong.TryParse(guildIdString.Trim(), out ulong guildId))
                        {
                            if (IsGuildIdValidBool(guildId))
                            {
                                Settings.GuildId = guildId;
                                SaveAndReloadSettingsDatabase();
                                IsGuildIdProcessComplete = true;
                            }
                            else
                            {
                                IsGuildIdProcessComplete= false;
                            }
                        }
                    }                   
                }
                else
                {
                    IsGuildIdProcessComplete = true;
                }
            }
        }

        public void SetGitBackupProcess()
        {
            bool IsGitBackupProcessComplete = false;
            while (!IsGitBackupProcessComplete)
            {
                if (!IsGitPatTokenSet())
                {
                    Console.WriteLine($"{DateTime.Now} SettingsManager - Enter your Git PAT Token now if you want to have online backup storage through a GitHub repo you control.\nIf you wish to skip this feature for now, enter 0 for the PAT token.\nRefer to documentation for more help with the Git Backup Storage.");
                    string? gitPatToken = Console.ReadLine();
                    if (!gitPatToken.Equals("0") && gitPatToken.Length > 15)
                    {
                        Settings.GitPatToken = gitPatToken;
                        Console.WriteLine($"{DateTime.Now} SettingsManager - Git PAT Token accepted. Now give the https url path to your Git repo. It will look something like this: https://github.com/YourUsername/YourGitStorageRepo.git");
                        string? gitUrlPath = Console.ReadLine();
                        Settings.GitUrlPath = gitUrlPath;
                        Console.WriteLine($"{DateTime.Now} SettingsManager - Repo Url set to: {gitUrlPath}\nYou can manually change your token and url path in the Settings/config.json file as well.");
                        SaveAndReloadSettingsDatabase();
                        IsGitBackupProcessComplete = true;
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} SettingsManager - Git Backup Storage was not set up. You can manually change your token and url path in the Settings/config.json file.");
                        IsGitBackupProcessComplete = true;
                    }
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now} SettingsManager - Non-default value found for GitPatToken in config.json file. Skipping backup setup process. If you entered in the token or url incorrectly, you can manually change it in the config.json file for now.");
                    IsGitBackupProcessComplete = true;
                }
            }
        }

        public void SetSuperAdminModeOnOff(bool trueOrFalse)
        {
            Settings.SuperAdminMode = trueOrFalse;
        }

        public void AddSuperAdminId(ulong discordID)
        {
            Settings.SuperAdminDiscordIds.Add(discordID);
        }

        public void RemoveSuperAdminId(ulong discordID)
        {
            Settings.SuperAdminDiscordIds.Remove(discordID);
        }
    }
}
