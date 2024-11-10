using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Settings
    {
        public string DiscordBotToken { get; set; } = "ENTER_BOT_TOKEN_HERE";
        public ulong GuildId { get; set; } = 0;
        public string CommandPrefix { get; set; } = "/";
        public bool SuperAdminMode { get; set; } = false;
        public List<ulong> SuperAdminDiscordIds { get; set; } = [];

        public Settings()
        { 

        }
    }
}
