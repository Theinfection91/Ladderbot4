using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Settings
    {
        public string DiscordBotToken { get; set; }
        public string CommandPrefix { get; set; }
        public bool SuperAdminMode { get; set; }
        public List<ulong> SuperAdminDiscordIds { get; set; }

        public Settings()
        { 

        }
    }
}
