using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Ladderbot4.Managers;

namespace Ladderbot4.Commands
{
    [Group("member", "Slash commands for member related information.")]
    public class MemberSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public MemberSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }
    }
}
