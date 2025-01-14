using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Ladderbot4.Managers;
using Ladderbot4.Models.Modals;

namespace Ladderbot4.Commands.ModalHandlers
{
    public class ModalInteractionHandlers : InteractionModuleBase<SocketInteractionContext>
    {
        private LadderManager _ladderManager;

        public ModalInteractionHandlers(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        #region Ladder End
        [ModalInteraction("ladder_end")]
        public async Task HandleEndLadderModalAsync(LadderEndModal modal)
        {
            string leagueNameOne = modal.LeagueNameOne;
            string leagueNameTwo = modal.LeagueNameTwo;

            // Compare league names
            if (leagueNameOne.Equals(leagueNameTwo, StringComparison.OrdinalIgnoreCase))
            {
                var result = _ladderManager.EndLeagueLadderProcess(leagueNameOne.Trim().ToLower());
                await RespondAsync(embed: result);
            }
        }
        #endregion
    }
}
