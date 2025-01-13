using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("report", "Slash commands related to report data, like report win")]
    public class ReportSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public ReportSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("win", "User-level command to report who won in a match.")]
        public async Task ReportWinAsync(string winningTeamName)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.ReportXvXWinProcess(Context, winningTeamName.Trim().ToLower());
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

            [SlashCommand("win", "Admin-level command to report who won in a match.")]
            [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
            public async Task ReportWinAdminAsync(string winningTeamName)
            {
                try
                {
                    await Context.Interaction.DeferAsync();
                    var result = _ladderManager.ReportXvXWinAdminProcess(Context, winningTeamName.Trim().ToLower());
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
