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
{   // Base Team Group
    [Group("team", "Slash commands related to team management.")]
    public class TeamSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public TeamSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        #region Register/Remove Team Commands
        [SlashCommand("register", "Admin command to register team in given division.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task RegisterTeamAsync(
            [Summary("teamName", "Name of the team to be registered")] string teamName,
            [Summary("division", "Division type (1v1, 2v2, 3v3)")] string divisionType,
            [Summary("member1", "For creating 1v1 team")] IUser member1,
            [Summary("member2", "For creating 2v2 team")] IUser? member2 = null,
            [Summary("member3", "For creating 3v3 team")] IUser? member3 = null)
        {
            // Compile member(s) to list
            var members = new List<IUser> { member1 };
            if (member2 != null) members.Add(member2);
            if (member3 != null) members.Add(member3);

            var result = _ladderManager.RegisterTeamProcess(Context, teamName, divisionType.Trim().ToLower(), members);
            await RespondAsync(embed: result);
        }

        [SlashCommand("remove", "Admin command to remove team from teams database.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task RemoveTeamAsync(
            [Summary("teamName", "Name of the team to be removed.")] string teamName)
        {
            var result = _ladderManager.RemoveTeamProcess(teamName);
            await RespondAsync(embed: result);
        }
        #endregion

        // Team Add Group
        [Group("add", "Slash commands to add wins/losses to teams.")]
        public class AddWinLossSlashCommands : InteractionModuleBase<SocketInteractionContext>
        {
            private readonly LadderManager _ladderManager;

            public AddWinLossSlashCommands(LadderManager ladderManager)
            {
                _ladderManager = ladderManager;
            }

            #region Add Win/Loss Commands
            [SlashCommand("win", "Admin command to add numberOfWins to given team")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task AddWinAsync(
                [Summary("teamName", "The name of the team to add wins to.")] string teamName,
                [Summary("numberOfWins", "The number of wins to add to the team.")] int numberOfWins)
            {
                var result = _ladderManager.AddToWinCountProcess(Context, teamName, numberOfWins);
                await RespondAsync(embed: result);
            }

            [SlashCommand("loss", "Admin command to add numberOfWins to given team")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task AddLossAsync(
                [Summary("teamName", "The name of the team to add losses to.")] string teamName,
                [Summary("numberOfLosses", "The number of losses to add to the team.")] int numberOfLosses)
            {
                var result = _ladderManager.AddToLossCountProcess(Context, teamName, numberOfLosses);
                await RespondAsync(embed: result);
            }
            #endregion
        }

        // Team Subtract Group
        [Group("subtract", "Slashs commands to subtract wins/losses from teams.")]
        public class SubtractWinLossComands : InteractionModuleBase<SocketInteractionContext>
        {
            private readonly LadderManager _ladderManager;

            public SubtractWinLossComands(LadderManager ladderManager)
            {
                _ladderManager = ladderManager;
            }

            #region Subtract Win/Loss Commands
            [SlashCommand("win", "Admin command to subtract numberOfWins from given team")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task SubtractWinAsync(
                [Summary("teamName", "The name of the team to subtract wins from.")] string teamName,
                [Summary("numerOfWins", "The number of wins to subtract from team.")] int numberOfWins)
            {
                var result = _ladderManager.SubtractFromWinCountProcess(Context, teamName, numberOfWins);
                await RespondAsync(embed: result);
            }

            [SlashCommand("loss", "Admin command to subtract numberOfLosses from given team")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task SubtractLossAsync(
                [Summary("teamName", "The name of the team to subtract losses from.")] string teamName,
                [Summary("numberOfLosses", "The number of losses to subtract from team.")] int numberOfLosses)
            {
                var result = _ladderManager.SubtractFromLossCountProcess(Context, teamName, numberOfLosses);
                await RespondAsync(embed: result);
            }
            #endregion
        }
    }
}
