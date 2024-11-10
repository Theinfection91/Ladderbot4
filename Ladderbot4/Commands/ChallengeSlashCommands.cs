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

        [SlashCommand("send", "Attempts to send a challenge from users given team to another.")]
        public async Task ChallengeAsync(
            [Summary("challengerTeam", "Name of team sending challenge")] string challengerTeam,
            [Summary("challengedTeam", "Name of team receiving challenge")] string challengedTeam)
        {
            string result = _ladderManager.ChallengeProcess(Context, challengerTeam, challengedTeam);
            await RespondAsync(result);
        }
    }
}
