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
    [Group("git", "Slash commands related to Git backup system.")]
    public class GitSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public GitSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        #region Create Branch Backup
        [SlashCommand("branch", "Admin command to create new branch of database in repo.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task BranchBackupDataAsync(
            [Summary("branchName", "Optional name of branch")] string branchName = "default")
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = "In-Progress";

                await Context.Interaction.FollowupAsync(result);
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
