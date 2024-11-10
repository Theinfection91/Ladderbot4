using Discord.Commands;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    public class StartEndChannelCommands : ModuleBase<SocketCommandContext>
    {
        private readonly LadderManager _ladderManager;

        public StartEndChannelCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [Command("end_ladder")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task EndLadderAsync(string division)
        {

        }

        [Command("set_challenges_channel")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetChallengesChannelAsync(string division)
        {

        }

        [Command("set_standings_channel")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetStandingsChannelAsync(string division)
        {

        }

        [Command("set_teams_channel")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetTeamsChannelAsync(string division)
        {

        }
    }
}
