﻿using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("ladder", "Commands related to overall ladder management (start/end, designated channels stats, etc.)")]
    public class LadderStatesSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public LadderStatesSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("start", "Starts the ladder in the given League if it's not already running.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task StartLadderAsync(
            [Summary("leagueName", "The League you want to start the ladder in.")]string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.StartLeagueLadderProcess(leagueName.Trim().ToLower());
                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("end", "Ends the ladder in the given League if it's not already running.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task EndLadderAsync(
            [Summary("leagueName", "The League you want to end the ladder in.")] string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.EndLeagueLadderProcess(leagueName.Trim().ToLower());
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
