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
        
        #region Ladder Start/End
        [ModalInteraction("ladder_start")]
        public async Task HandleLadderModalAsync(LadderStartModal modal)
        {
            string leagueNameOne = modal.LeagueNameOne;
            string leagueNameTwo = modal.LeagueNameTwo;

            // Compare league names user has given
            if (leagueNameOne.Equals(leagueNameTwo))
            {
                // Check if league is in database. Case sensitive.
                if (!_leagueManager.IsLeagueNameUnique(leagueNameOne, true))
                {
                    // Init end ladder process
                    Embed result = _ladderManager.StartLeagueLadderProcess(leagueNameOne.Trim().ToLower());
                    await RespondAsync(embed: result);
                }
                else
                {
                    Embed result = _embedManager.LadderModalErrorEmbed($"The given league name of `{leagueNameOne}` does not exist in the database. Check your spelling and capitalization as the Start Ladder confirmation process is case sensitive and your input must match the league name exactly.", "Start");
                    await RespondAsync(embed: result, ephemeral: true);
                }
            }
            else
            {
                Embed result = _embedManager.LadderModalErrorEmbed($"The given league names did not match. The Start Ladder confirmation process is case sensitive so check your spelling and capitalization. Your input entries:\n\t1 - {leagueNameOne}\n\t2 - {leagueNameTwo}", "Start");
                await RespondAsync(embed: result, ephemeral: true);
            }
        }
        
        [ModalInteraction("ladder_end")]
        public async Task HandleLadderModalAsync(LadderEndModal modal)
        {
            string leagueNameOne = modal.LeagueNameOne;
            string leagueNameTwo = modal.LeagueNameTwo;

            // Compare league names user has given
            if (leagueNameOne.Equals(leagueNameTwo))
            {
                // Check if league is in database. Case sensitive.
                if (!_leagueManager.IsLeagueNameUnique(leagueNameOne, true))
                {
                    // Init end ladder process
                    Embed result = _ladderManager.EndLeagueLadderProcess(leagueNameOne.Trim().ToLower());
                    await RespondAsync(embed: result);
                }
                else
                {
                    Embed result = _embedManager.LadderModalErrorEmbed($"The given league name of `{leagueNameOne}` does not exist in the database. Check your spelling and capitalization as the End Ladder confirmation process is case sensitive and your input must match the league name exactly.", "End");
                    await RespondAsync(embed: result, ephemeral: true);
                }
            }
            else
            {
                Embed result = _embedManager.LadderModalErrorEmbed($"The given league names did not match. The End Ladder confirmation process is case sensitive so check your spelling and capitalization. Your input entries:\n\t1 - {leagueNameOne}\n\t2 - {leagueNameTwo}", "End");
                await RespondAsync(embed: result, ephemeral: true);
            }
        }
        #endregion

        #region League Delete
        [ModalInteraction("league_delete")]
        public async Task HandleLeagueModalAsync(LeagueDeleteModal modal)
        {
            string leagueNameOne = modal.LeagueNameOne;
            string leagueNameTwo = modal.LeagueNameTwo;

            // Compare league names user has given
            if (leagueNameOne.Equals(leagueNameTwo))
            {
                // Check if league is in database. Case sensitive.
                if (!_leagueManager.IsLeagueNameUnique(leagueNameOne, true))
                {
                    // Init end ladder process
                    Embed result = _ladderManager.DeleteLeagueProcess(leagueNameOne.Trim().ToLower());
                    await RespondAsync(embed: result);
                }
                else
                {
                    Embed result = _embedManager.LeagueModalErrorEmbed($"The given league name of `{leagueNameOne}` does not exist in the database. Check your spelling and capitalization as the Delete League confirmation process is case sensitive and your input must match the league name exactly.");
                    await RespondAsync(embed: result, ephemeral: true);
                }
            }
            else
            {
                Embed result = _embedManager.LeagueModalErrorEmbed($"The given league names did not match. The Delete League confirmation process is case sensitive so check your spelling and capitalization. Your input entries:\n\t1 - {leagueNameOne}\n\t2 - {leagueNameTwo}");
                await RespondAsync(embed: result, ephemeral: true);
            }
        }
        #endregion

        #region Team Remove
        [ModalInteraction("team_remove")]
        public async Task HandleTeamModalAsync(TeamRemoveModal modal)
        {
            string TeamNameOne = modal.TeamNameOne;
            string TeamNameTwo = modal.TeamNameTwo;

            // Compare league names user has given
            if (TeamNameOne.Equals(TeamNameTwo))
            {
                // Check if league is in database. Case sensitive.
                if (!_leagueManager.IsTeamNameUnique(TeamNameOne, true))
                {
                    // Init end ladder process
                    Embed result = _ladderManager.RemoveTeamFromLeagueProcess(TeamNameOne.Trim().ToLower());
                    await RespondAsync(embed: result);
                }
                else
                {
                    Embed result = _embedManager.TeamModalErrorEmbed($"The given team name of `{TeamNameOne}` does not exist in the database. Check your spelling and capitalization as the Remove Team confirmation process is case sensitive and your input must match the team name exactly.");
                    await RespondAsync(embed: result, ephemeral: true);
                }
            }
            else
            {
                Embed result = _embedManager.TeamModalErrorEmbed($"The given team names did not match. The Remove Team confirmation process is case sensitive so check your spelling and capitalization. Your input entries:\n\t1 - {TeamNameOne}\n\t2 - {TeamNameTwo}");
                await RespondAsync(embed: result, ephemeral: true);
            }
        }
        #endregion
    }
}
