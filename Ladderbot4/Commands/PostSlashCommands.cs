using Discord.Interactions;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("post", "Slash commands for posting standings/challenges/teams.")]
    public class PostSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public readonly LadderManager _ladderManager;

        public PostSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("challenges", "Slash command for posting challenges of given division.")]
        public async Task PostChallengesAsync(
            [Summary("division", "The division in which to post data from.")] string division)
        {
            //var result = _ladderManager.PostChallengesProcess(Context, division.Trim().ToLower());
            //await RespondAsync(embed: result);
        }

        [SlashCommand("standings", "Slash command for posting standings of given League.")]
        public async Task PostStandingsAsync(
            [Summary("leagueName", "The League in which to post data from.")] string leagueName)
        {
            await Context.Interaction.DeferAsync();

            var result = _ladderManager.PostStandingsProcess(Context, leagueName.Trim().ToLower());

            await Context.Interaction.FollowupAsync(embed: result);
        }

        [SlashCommand("teams", "Slash commands for posting teams of given division")]
        public async Task PostTeamsAsync(
            [Summary("division", "The division in which to post data from.")] string division)
        {
            //var result = _ladderManager.PostTeamsProcess(Context, division.Trim().ToLower());
            //await RespondAsync(embed: result);
        }
    }
}
