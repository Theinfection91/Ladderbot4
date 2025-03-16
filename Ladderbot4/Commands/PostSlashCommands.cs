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
    [Group("post", "Slash commands for posting standings/challenges/teams.")]
    public class PostSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public PostSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("challenges", "Slash command for posting challenges of given division.")]
        public async Task PostChallengesAsync(
            [Summary("league_name", "The League in which to post challenge data from."), Autocomplete] string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.PostChallengesProcess(Context, leagueName.Trim().ToLower());

                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("leagues", "Slash command for posting all leagues or all of given division type")]
        public async Task PostLeaguesAsync()
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.PostLeaguesProcess(Context);

                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {

                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("standings", "Slash command for posting standings of given League.")]
        public async Task PostStandingsAsync(
            [Summary("league_name", "The League in which to post standings data from."), Autocomplete] string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.PostStandingsProcess(Context, leagueName.Trim().ToLower());

                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("teams", "Slash commands for posting teams of given division")]
        public async Task PostTeamsAsync(
            [Summary("league_name", "The League in which to post teams data from."), Autocomplete] string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.PostTeamsProcess(Context, leagueName.Trim().ToLower());

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
