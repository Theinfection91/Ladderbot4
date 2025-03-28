﻿using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Managers;
using Ladderbot4.Models.Modals;
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

        [SlashCommand("start", "Load confirmation modal to begin Start Ladder process.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task StartLadderModalAsync()
        {
            try
            {
                await RespondWithModalAsync<LadderStartModal>("ladder_start");
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await FollowupAsync(embed: errorResult, ephemeral: true);
            }
        }

        [SlashCommand("end", "Load confirmation modal to begin End Ladder process.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task EndLadderModalAsync()
        {
            try
            {
                await RespondWithModalAsync<LadderEndModal>("ladder_end");
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await FollowupAsync(embed: errorResult, ephemeral: true);
            }
        }
    }
}
