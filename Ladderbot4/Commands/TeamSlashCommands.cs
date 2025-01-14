using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Managers;
using Ladderbot4.Models;
using Ladderbot4.Models.Modals;
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
        [SlashCommand("register", "Admin command to register a team in the specified league.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task RegisterXvXTeamAsync(
            [Summary("teamName", "Name of the team to be registered")] string teamName,
            [Summary("leagueName", "The league to register the team to")] string leagueName,
            [Summary("member1", "A member to add to the team.")] IUser member1,
            [Summary("member2", "A member to add to the team.")] IUser? member2 = null,
            [Summary("member3", "A member to add to the team.")] IUser? member3 = null,
            [Summary("member4", "A member to add to the team.")] IUser? member4 = null,
            [Summary("member5", "A member to add to the team.")] IUser? member5 = null,
            [Summary("member6", "A member to add to the team.")] IUser? member6 = null,
            [Summary("member7", "A member to add to the team.")] IUser? member7 = null,
            [Summary("member8", "A member to add to the team.")] IUser? member8 = null,
            [Summary("member9", "A member to add to the team.")] IUser? member9 = null,
            [Summary("member10", "A member to add to the team.")] IUser? member10 = null,
            [Summary("member11", "A member to add to the team.")] IUser? member11 = null,
            [Summary("member12", "A member to add to the team.")] IUser? member12 = null,
            [Summary("member13", "A member to add to the team.")] IUser? member13 = null,
            [Summary("member14", "A member to add to the team.")] IUser? member14 = null,
            [Summary("member15", "A member to add to the team.")] IUser? member15 = null,
            [Summary("member16", "A member to add to the team.")] IUser? member16 = null,
            [Summary("member17", "A member to add to the team.")] IUser? member17 = null,
            [Summary("member18", "A member to add to the team.")] IUser? member18 = null,
            [Summary("member19", "A member to add to the team.")] IUser? member19 = null,
            [Summary("member20", "A member to add to the team.")] IUser? member20 = null)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                // Initialize the list of members
                var members = new List<IUser>() { member1 };

                // Add members to the list if they are not null
                if (member2 != null) members.Add(member2);
                if (member3 != null) members.Add(member3);
                if (member4 != null) members.Add(member4);
                if (member5 != null) members.Add(member5);
                if (member6 != null) members.Add(member6);
                if (member7 != null) members.Add(member7);
                if (member8 != null) members.Add(member8);
                if (member9 != null) members.Add(member9);
                if (member10 != null) members.Add(member10);
                if (member11 != null) members.Add(member11);
                if (member12 != null) members.Add(member12);
                if (member13 != null) members.Add(member13);
                if (member14 != null) members.Add(member14);
                if (member15 != null) members.Add(member15);
                if (member16 != null) members.Add(member16);
                if (member17 != null) members.Add(member17);
                if (member18 != null) members.Add(member18);
                if (member19 != null) members.Add(member19);
                if (member20 != null) members.Add(member20);

                var result = _ladderManager.RegisterTeamToLeagueProcess(Context, teamName.Trim(), leagueName.Trim().ToLower(), members);

                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("remove", "Load confirmation modal to begin Remove Team process.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task StartTeamModalAsync()
        {
            try
            {
                await RespondWithModalAsync<TeamRemoveModal>("team_remove");
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await FollowupAsync(embed: errorResult, ephemeral: true);
            }
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
                try
                {
                    await Context.Interaction.DeferAsync();
                    var result = _ladderManager.AddToWinCountProcess(Context, teamName.Trim().ToLower(), numberOfWins);
                    await Context.Interaction.FollowupAsync(embed: result);
                }
                catch (Exception ex)
                {
                    string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                    var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                    await Context.Interaction.FollowupAsync(embed: errorResult);
                }
            }

            [SlashCommand("loss", "Admin command to add numberOfWins to given team")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task AddLossAsync(
                [Summary("teamName", "The name of the team to add losses to.")] string teamName,
                [Summary("numberOfLosses", "The number of losses to add to the team.")] int numberOfLosses)
            {
                try
                {
                    await Context.Interaction.DeferAsync();
                    var result = _ladderManager.AddToLossCountProcess(Context, teamName.Trim().ToLower(), numberOfLosses);
                    await Context.Interaction.FollowupAsync(embed: result);
                }
                catch (Exception ex)
                {
                    string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                    var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                    await Context.Interaction.FollowupAsync(embed: errorResult);
                }
            }           
        }
        #endregion

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
                try
                {
                    await Context.Interaction.DeferAsync();
                    var result = _ladderManager.SubtractFromWinCountProcess(Context, teamName.Trim().ToLower(), numberOfWins);
                    await Context.Interaction.FollowupAsync(embed: result);
                }
                catch (Exception ex)
                {
                    string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                    var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                    await Context.Interaction.FollowupAsync(embed: errorResult);
                }
            }

            [SlashCommand("loss", "Admin command to subtract numberOfLosses from given team")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task SubtractLossAsync(
                [Summary("teamName", "The name of the team to subtract losses from.")] string teamName,
                [Summary("numberOfLosses", "The number of losses to subtract from team.")] int numberOfLosses)
            {
                try
                {
                    await Context.Interaction.DeferAsync();
                    var result = _ladderManager.SubtractFromLossCountProcess(Context, teamName.Trim().ToLower(), numberOfLosses);
                    await Context.Interaction.FollowupAsync(embed: result);
                }
                catch (Exception ex)
                {
                    string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                    var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                    await Context.Interaction.FollowupAsync(embed: errorResult);
                }
            }
            #endregion
        }
    }
}
