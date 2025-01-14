using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Ladderbot4.Managers;
using Ladderbot4.Models.Modals;

namespace Ladderbot4.Commands.ModalHandlers
{
    public class ModalInteractionHandlers : InteractionModuleBase<SocketInteractionContext>
    {
        private EmbedManager _embedManager;

        private LadderManager _ladderManager;

        private LeagueManager _leagueManager;

        public ModalInteractionHandlers(EmbedManager embedManager, LadderManager ladderManager, LeagueManager leagueManager)
        {
            _embedManager = embedManager;
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
                    Embed result = _ladderManager.EndLeagueLadderProcess(leagueNameOne.Trim().ToLower());
                    await RespondAsync(embed: result);
                }
                else
                {
                    Embed result = _embedManager.EndLadderModalErrorEmbed($"The given league name of `{leagueNameOne}` does not exist in the database. Check your spelling and capitalization as the End Ladder confirmation process is case sensitive and your input must match the league name exactly.");
                    await RespondAsync(embed: result, ephemeral: true);
                }
            }
            else
            {
                Embed result = _embedManager.EndLadderModalErrorEmbed($"The given league names did not match. The End Ladder confirmation process is case sensitive so check your spelling and capitalization. Your input entries:\n\t1 - {leagueNameOne}\n\t2 - {leagueNameTwo}");
                await RespondAsync(embed: result, ephemeral: true);
            }
        }
        #endregion
    }
}
