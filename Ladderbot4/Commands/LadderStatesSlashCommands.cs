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

        [SlashCommand("start", "Starts the ladder in the given division type if it's not already running.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task StartLadderAsync(
            [Summary("division", "The division of Ladder to try and start (1v1, 2v2, 3v3)")]string division)
        {
            // Initiate Logic from LadderManager
            var result = _ladderManager.StartLadderByDivisionProcess(division.Trim().ToLower());

            // Send the response
            await RespondAsync(embed: result);
        }

        [SlashCommand("end", "Ends the ladder in the given division type if it's not already running.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task EndLadderAsync(
            [Summary("division", "The division of Ladder to try and end (1v1, 2v2, 3v3)")] string division)
        {
            // Initiate Logic from LadderManager
            var result = _ladderManager.EndLadderByDivisionProcess(division.Trim().ToLower());

            // Send the response
            await RespondAsync(embed: result);
        }
    }
}
