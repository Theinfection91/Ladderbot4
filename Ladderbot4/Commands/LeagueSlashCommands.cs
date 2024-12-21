﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Ladderbot4.Managers;

namespace Ladderbot4.Commands
{
    [Group("league", "Slash commands related to creation/removal of Leagues")]
    public class LeagueSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public LeagueSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        #region Create/Delete League Commands
        [SlashCommand("create", "Admin command to create a new League of the given divison type")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task CreateLeagueAsync(
            [Summary("leagueName", "Name of the League to be created")] string leagueName,
            [Summary("divisionType", "Division type (1v1, 2v2, 3v3)")] string divisionType)
        {
            // Defer response if the process might take time
            await Context.Interaction.DeferAsync();

            var result = _ladderManager.CreateLeagueProcess(leagueName, divisionType.Trim().ToLower());

            // Send the resulting embed
            await Context.Interaction.FollowupAsync(embed: result);
        }

        [SlashCommand("delete", "Admin command to delete a League entirely. Use with caution.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task DeleteLeagueAsync(
            [Summary("leagueName", "Name of the League to be deleted")] string leagueName)
        {
            // Defer response if the process might take time
            await Context.Interaction.DeferAsync();

            var result = _ladderManager.DeleteLeagueProcess(leagueName);

            // Send the resulting embed
            await Context.Interaction.FollowupAsync(embed: result);
        }
        #endregion
    }
}
