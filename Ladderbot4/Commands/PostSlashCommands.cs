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
        public readonly LadderManager _ladderManager;

        public PostSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("challenges", "Slash command for posting challenges of given division.")]
        public async Task PostChallengesAsync(
            [Summary("leagueName", "The League in which to post challenge data from.")] string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.PostXvXChallengesProcess(Context, leagueName.Trim().ToLower());

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
        public async Task PostLeaguesAsync(
            [Summary("leagueFormat", "If given a leagueFormat, will post all of that type.")] string leagueFormat = "all")
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.PostXvXLeaguesProcess(Context, leagueFormat.Trim().ToLower());

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
            [Summary("leagueName", "The League in which to post standings data from.")] string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.PostXvXStandingsProcess(Context, leagueName.Trim().ToLower());

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
            [Summary("leagueName", "The League in which to post teams data from.")] string leagueName)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.PostXvXTeamsProcess(Context, leagueName.Trim().ToLower());

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
