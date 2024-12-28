using Discord;
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
    [Group("set", "Commands relating to set like rank and division text channels.")]
    public class SetSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public SetSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("rank", "Sets the specified rank to the given team")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetRankAsync(
            [Summary("teamName", "The team that will have their rank changed")] string teamName,
            [Summary("rank", "The new rank the team will be awarded.")] int rank)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.SetRankProcess(Context, teamName.Trim().ToLower(), rank);

                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("challenges_channel_id", "For Admins to set the dynamic challenges message.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetChallengesChannelIdAsync(
            [Summary("leagueName", "League name for challenges data.")] string leagueName,
            [Summary("channel", "The text channel to set to.")] IMessageChannel channel)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.SetChallengesChannelIdProcess(leagueName, channel);

                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("standings_channel_id", "For Admins to set the dynamic standings message.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetStandingsChannelIdAsync(
            [Summary("leagueName", "League name for standings data.")] string leagueName,
            [Summary("channel", "The text channel to set to.")] IMessageChannel channel)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.SetStandingsChannelIdProcess(leagueName, channel);

                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("teams_channel_id", "For Admins to set the dynamic teams message.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetTeamsChannelIdAsync(
            [Summary("leagueName", "League name for teams data.")] string leagueName,
            [Summary("channel", "The text channel to set to.")] IMessageChannel channel)
        {
            try
            {
                await Context.Interaction.DeferAsync();

                var result = _ladderManager.SetTeamsChannelIdProcess(leagueName, channel);

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
