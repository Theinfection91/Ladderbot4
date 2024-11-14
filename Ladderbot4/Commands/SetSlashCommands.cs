using Discord.Interactions;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("set", "Commands relating to set like rank and division text channels.")]
    public  class SetSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public SetSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("rank", "Sets the specified rank to the given team")]
        public async Task SetRankAsync(
            [Summary("teamName", "The team that will have their rank changed")] string teamName,
            [Summary("rank", "The new rank the team will be awarded.")] int rank)
        {
            string result = _ladderManager.SetRankProcess(teamName, rank);
            await RespondAsync(result);
        }
    }
}
