using Discord.Interactions;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("test", "Slash commands related to testing purposes")]
    public class TestSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public TestSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("achieve", "Manual Achievement testing")]
        public async Task TestAchieveAsync(string winningTeamName)
        {
            string result = "";
            await RespondAsync(result);
        }
    }
}
