using Discord;
using Discord.WebSocket;
using Ladderbot4.Data;
using Ladderbot4.Models;
using Ladderbot4.Models.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class AchievementManager
    {
        private DiscordSocketClient _client;

        public AchievementManager(DiscordSocketClient client)
        {
            _client = client;
        }
    }
}
