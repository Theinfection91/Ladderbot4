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

        [Command("test")]
        public async Task TestAsync()
        {
            Console.WriteLine("Test command invoked");
            await ReplyAsync("Test");
        }
    }
}