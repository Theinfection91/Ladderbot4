using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Ladderbot4.Models.Modals;

namespace Ladderbot4.Commands.ModalHandlers
{
    public class LadderModalHandlers : InteractionModuleBase<SocketInteractionContext>
    {
        public LadderModalHandlers() { }

        [ModalInteraction("ladder_end")]
        public async Task HandleEndLadderModalAsync(LadderEndModal modal)
        {
            try
            {
                // Defer the response
                await DeferAsync(ephemeral: true);

                string leagueNameOne = modal.LeagueNameOne;
                string leagueNameTwo = modal.LeagueNameTwo;

                // Compare league names
                if (leagueNameOne.Equals(leagueNameTwo, StringComparison.OrdinalIgnoreCase))
                {
                    //var result = _ladderManager.EndLeagueLadderProcess(leagueNameOne);
                    await FollowupAsync($"The ladder for **{leagueNameOne}** has been successfully ended.", ephemeral: true);
                }
                else
                {
                    await FollowupAsync("The league names do not match. Please try again.", ephemeral: true);
                }
            }
            catch (Exception ex)
            {
                await FollowupAsync($"An error occurred: {ex.Message}", ephemeral: true);
            }
        }
    }
}
