using Discord.Interactions;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("post", "Slash commands for posting standings/challenges/teams.")]
    public class PostSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public readonly LadderManager _ladderManager;

        public PostSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("standings", "Slash command for posting standings of given division.")]
        public async Task PostStandingsAsync(
            [Summary("division", "The division in which to post data from.")] string division)
        {
            string result = _ladderManager.PostStandingsProcess(Context, division);
            await RespondAsync(result);
        }
    }
}
