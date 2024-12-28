using Discord.Interactions;
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
            await Context.Interaction.DeferAsync();

            // Initiate Logic from LadderManager
            var result = _ladderManager.StartLeagueLadderProcess(leagueName.Trim().ToLower());

            // Send the response
            await Context.Interaction.FollowupAsync(embed: result);
        }

        [SlashCommand("end", "Ends the ladder in the given League if it's not already running.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task EndLadderAsync(
            [Summary("leagueName", "The League you want to end the ladder in.")] string leagueName)
        {
            await Context.Interaction.DeferAsync();

            // Initiate Logic from LadderManager
            var result = _ladderManager.EndLeagueLadderProcess(leagueName.Trim().ToLower());

            // Send the response
            await Context.Interaction.FollowupAsync(embed: result);
        }
    }
}
