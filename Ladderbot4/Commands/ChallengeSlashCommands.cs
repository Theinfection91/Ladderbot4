﻿using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Managers;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("challenge", "Slash commands related to challenges.")]
    public class ChallengeSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public ChallengeSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("send", "Attempts to send a challenge from invoker's team they are on to another team.")]
        public async Task SendChallengeAsync(
            [Summary("challenger_team", "Name of team sending challenge"), Autocomplete] string challengerTeam,
            [Summary("challenged_team", "Name of team receiving challenge"), Autocomplete] string challengedTeam)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.SendChallengeProcess(Context, challengerTeam.Trim().ToLower(), challengedTeam.Trim().ToLower());
                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("cancel", "Attempts to cancel a challenge from invoker's team they are on to another team.")]
        public async Task CancelChallengeAsync(
            [Summary("challenger_team", "Name of team that sent the challenge"), Autocomplete] string challengerTeam)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.CancelChallengeProcess(Context, challengerTeam.Trim().ToLower());
                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [Group("admin", "Admin slash commands related to challenges.")]
        public class AdminChallengeSlashCommands : InteractionModuleBase<SocketInteractionContext>
        {
            private readonly LadderManager _ladderManager;

            public AdminChallengeSlashCommands(LadderManager ladderManager)
            {
                _ladderManager = ladderManager;
            }

            [SlashCommand("send", "Attempts to send a challenge from one team to another team as Admin.")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task ChallengeAsync(
            [Summary("challenger_team", "Name of challenger team"), Autocomplete] string challengerTeam,
            [Summary("challenged_team", "Name of team receiving challenge"), Autocomplete] string challengedTeam)
            {
                try
                {
                    await Context.Interaction.DeferAsync();
                    var result = _ladderManager.SendAdminChallengeProcess(Context, challengerTeam.Trim().ToLower(), challengedTeam.Trim().ToLower());
                    await Context.Interaction.FollowupAsync(embed: result);
                }
                catch (Exception ex)
                {
                    string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                    var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                    await Context.Interaction.FollowupAsync(embed: errorResult);
                }
            }

            [SlashCommand("cancel", "Attempts to cancel a challenge from a challenger team as Admin.")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task CancelChallengeAsync(
            [Summary("challenger_team", "Name of challenger team"), Autocomplete] string challengerTeam)
            {
                try
                {
                    await Context.Interaction.DeferAsync();
                    var result = _ladderManager.AdminCancelChallengeProcess(Context, challengerTeam.Trim().ToLower());
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
    }
}
