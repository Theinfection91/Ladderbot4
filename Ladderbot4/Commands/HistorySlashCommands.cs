using Discord.Interactions;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("history", "Slash commands related to History (Past Matches)")]
    public class HistorySlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public HistorySlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }
    }
}
