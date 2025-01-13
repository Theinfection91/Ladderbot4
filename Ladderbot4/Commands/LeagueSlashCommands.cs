using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
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

        [SlashCommand("create", "Admin command to create a new XvX League with the given team size.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task CreateXvXLeagueAsync(
            [Summary("leagueName", "Name of the League to be created")] string leagueName,
            [Summary("teamSize", "The size of each team")] int teamSize)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.CreateLeagueProcess(leagueName.Trim(), teamSize);
                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }

        }

        [SlashCommand("delete", "Admin command to delete an XvX League entirely. Use with caution.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task DeleteXvXLeagueAsync(
            [Summary("leagueName", "Name of the League to be deleted")] string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.DeleteLeagueProcess(leagueName.Trim().ToLower());
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
