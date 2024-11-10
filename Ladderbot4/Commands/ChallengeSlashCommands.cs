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

        [SlashCommand("send", "Attempts to send a challenge from invoker's team they are on to another team.")]
        public async Task ChallengeAsync(
            [Summary("challengerTeam", "Name of team sending challenge")] string challengerTeam,
            [Summary("challengedTeam", "Name of team receiving challenge")] string challengedTeam)
        {
            string result = _ladderManager.ChallengeProcess(Context, challengerTeam, challengedTeam);
            await RespondAsync(result);
        }

        [SlashCommand("cancel", "Attempts to cancel a challenge from invoker's team they are on to another team.")]
        public async Task CancelChallengeAsync(
            [Summary("challengerTeam", "Name of team that sent challenge")] string challengerTeam)
        {
            string result = _ladderManager.CancelChallengeProcess(Context, challengerTeam);
            await RespondAsync(result);
        }
    }
}
