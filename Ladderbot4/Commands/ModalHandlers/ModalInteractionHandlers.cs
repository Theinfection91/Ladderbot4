using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Interactions;
using Ladderbot4.Managers;
using Ladderbot4.Models.Modals;

namespace Ladderbot4.Commands.ModalHandlers
{
    public class ModalInteractionHandlers : InteractionModuleBase<SocketInteractionContext>
    {
        private LadderManager _ladderManager;

        private LeagueManager _leagueManager;

        public ModalInteractionHandlers(LadderManager ladderManager, LeagueManager leagueManager)
        {
            _ladderManager = ladderManager;
            _leagueManager = leagueManager;
        }

        #region Ladder End
        [ModalInteraction("ladder_end")]
        public async Task HandleEndLadderModalAsync(LadderEndModal modal)
        {
            string leagueNameOne = modal.LeagueNameOne;
            string leagueNameTwo = modal.LeagueNameTwo;

            // Compare league names user has given
            if (leagueNameOne.Equals(leagueNameTwo))
            {
                // Check if league is in database. Case sensitive.
                if (!_leagueManager.IsLeagueNameUnique(leagueNameOne, true))
                {
                    // Init end ladder process with case-insensitive arguments now like normal
                    var result = _ladderManager.EndLeagueLadderProcess(leagueNameOne.Trim().ToLower());
                    await RespondAsync(embed: result);
                }
                else
                {
                    await RespondAsync("A League by the given name was not found in the database. Check your spelling and try again.", ephemeral: true);
                }
            }
            else
            {
                await RespondAsync("Given League names did not match. Input is case-sensitive. Try again.", ephemeral: true);
            }
        }
        #endregion
    }
}
