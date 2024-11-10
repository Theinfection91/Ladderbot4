using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Managers;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("team", "Slash commands related to team management.")]
    public class TeamSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public TeamSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("register", "Admin command to register team in given division.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task RegisterTeamAsync([Summary("teamName", "Name of the team to be registered")] string teamName, [Summary("division", "Division type (1v1, 2v2, 3v3)")] string divisionType, [Summary("member1", "For creating 1v1 team")] IUser member1, [Summary("member2", "For creating 2v2 team")] IUser? member2 = null, [Summary("member3", "For creating 3v3 team")] IUser? member3 = null)
        {
            // Compile member(s) to list
            var members = new List<IUser> { member1 };
            if (member2 != null) members.Add(member2);
            if (member3 != null) members.Add(member3);

            string result = _ladderManager.RegisterTeamProcess(teamName, divisionType, members);
            await RespondAsync(result);
        }
    }
}
