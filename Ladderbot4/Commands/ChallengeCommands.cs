using Discord.Commands;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    public class ChallengeCommands : ModuleBase<SocketCommandContext>
    {
        private readonly LadderManager _ladderManager;

        public ChallengeCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [Command("challenge", Aliases = ["chal"])]
        public async Task ChallengeAsync(string challengerTeam, string challengedTeam)
        {
            string result = _ladderManager.ChallengeProcess(Context, challengerTeam, challengedTeam);
            await ReplyAsync(result);
        }

        [Command("cancel_challenge", Aliases = ["cchal"])]
        public async Task CancelChallengeAsync(string challengerTeam)
        {
            string result = _ladderManager.CancelChallengeProcess(Context, challengerTeam);
            await ReplyAsync(result);
        }
    }
}
