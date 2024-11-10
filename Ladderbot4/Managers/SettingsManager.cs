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

        public void SaveSettings(Settings settings)
        {
            _settingsData.SaveSettings(settings);
        }

        public bool IsValidBotTokenSet()
        {
            return !string.IsNullOrEmpty(Settings.DiscordBotToken) && Settings.DiscordBotToken != "ENTER_BOT_TOKEN_HERE" && IsValidBotToken(Settings.DiscordBotToken);
        }

        public bool IsValidBotToken(string botToken)
        {
            return botToken.Length >= 59;
        }

        public bool IsGuildIdSet()
        {
            return Settings.GuildId != 0 && IsGuildIdValid();
        }

        public bool IsGuildIdValid()
        {
            return Settings.GuildId >= 15; 
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
                        SaveSettings(Settings);
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
    }
}
