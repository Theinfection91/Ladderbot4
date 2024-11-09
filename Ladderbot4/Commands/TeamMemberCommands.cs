using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    public class TeamMemberCommands : ModuleBase<SocketCommandContext>
    {
        public TeamMemberCommands()
        {
            Console.WriteLine("TeamMemberCommands module loaded.");
        }

        [Command("register_team", Aliases = ["regt"])]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)] // Administrator permission check
        public async Task RegisterTeam(string teamName, string divisionType, params SocketGuildUser[] members)
        {
            // Get the Discord ID of the person who invokes the command
            ulong userID = Context.User.Id;
            await ReplyAsync("Test");
        }

        [Command("test")]
        public async Task TestAsync()
        {
            Console.WriteLine("Test command invoked");
            await ReplyAsync("Test");
        }
    }
}
