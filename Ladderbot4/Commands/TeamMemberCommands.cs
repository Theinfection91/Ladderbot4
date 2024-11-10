using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    public class TeamMemberCommands : ModuleBase<SocketCommandContext>
    {
        private readonly LadderManager _ladderManager;

        public TeamMemberCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [Command("register_team", Aliases = ["regt"])]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)] // Administrator permission check
        public async Task RegisterTeamAsync(string teamName, string divisionType, params SocketGuildUser[] members)
        {
            string result = _ladderManager.RegisterTeamProcess(teamName, divisionType, members);
            await ReplyAsync(result);
        }

        [Command("test")]
        public async Task TestAsync()
        {
            Console.WriteLine("Test command invoked");
            await ReplyAsync("Test");
        }
    }
}