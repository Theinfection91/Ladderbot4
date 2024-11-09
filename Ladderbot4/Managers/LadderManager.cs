using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class LadderManager
    {
        public TeamManager TeamManager { get; set; }

        public LadderManager(TeamManager teamManager)
        {
            TeamManager = teamManager;
        }

        public void RegisterTeamProcess(string teamName, string divisionType, params SocketGuildUser[] members)
        {

        }
    }
}
