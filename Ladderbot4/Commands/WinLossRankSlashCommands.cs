using Discord.Interactions;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    public class WinLossRankSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public WinLossRankSlashCommands(LadderManager ladderManager)
        { 
            _ladderManager = ladderManager;
        }
    }
}
