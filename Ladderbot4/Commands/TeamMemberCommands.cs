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
        private LadderManager _ladderManager;

        public TeamMemberCommands(LadderManager ladderManager)
        {
            Console.WriteLine($"TeamMemberCommands module loaded with {ladderManager} passed through.");
        }

        [Command("register_team", Aliases = ["regt"])]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)] // Administrator permission check
        public async Task RegisterTeamAsync(string teamName, string divisionType, params SocketGuildUser[] members)
        {
            // Get the Discord ID of the person who invokes the command
            // ulong userID = Context.User.Id;
            
        }

        [Command("test")]
        public async Task TestAsync()
        {
            Console.WriteLine("Test command invoked");
            await ReplyAsync("Test");
        }
    }
}
