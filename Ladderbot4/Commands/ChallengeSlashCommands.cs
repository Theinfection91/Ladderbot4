using Discord.Interactions;
using Ladderbot4.Managers;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("challenge", "Slash commands related to team management.")]
    public class ChallengeSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public ChallengeSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("ping", "Replies with pong.")]
        public async Task PingCommand()
        {
            await RespondAsync("Pong!");
        }
    }
}
