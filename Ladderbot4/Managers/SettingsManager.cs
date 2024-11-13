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
        private readonly SettingsData _settingsData;
        public Settings Settings { get; set; }

        public SettingsManager(SettingsData settingsData)
        {
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
                        SaveSettings();
                        LoadSettingsData();
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
                    Console.WriteLine("Please select a guild from the list below: ");
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
