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
            return Settings.GuildId != 0;
        }

        public void SetBotTokenProcess()
        {
            bool IsBotTokenProcessComplete = false;
            while (!IsBotTokenProcessComplete)
            {
                if (!IsValidBotTokenSet())
                {
                    Console.WriteLine("Incorrect Bot Token found in Settings\\config.json");
                    Console.WriteLine("Please enter your Bot Token now (This can be changed manually in Settings\\config.json as well if entered incorrectly): ");
                    string? botToken = Console.ReadLine();
                    if (IsValidBotToken(botToken))
                    {
                        Settings.DiscordBotToken = botToken;
                        IsBotTokenProcessComplete = true;
                    }
                    else
                    {
                        IsBotTokenProcessComplete = false;
                    }
                }
            }
        }
    }
}
