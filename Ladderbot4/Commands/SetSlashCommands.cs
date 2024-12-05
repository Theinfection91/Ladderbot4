using Discord;
using Discord.Interactions;
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
            string result = _ladderManager.SetRankProcess(teamName, rank);
            await RespondAsync(result);
        }

        [SlashCommand("challenges_channel_id", "For Admins to set the dynamic challenges message.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetChallengesChannelIdAsync(
            [Summary("division", "Which division channel to set.")] string division,
            [Summary("channel", "The text channel to set to.")] IMessageChannel channel)
        {
            var result = _ladderManager.SetChallengesChannelIdProcess(division, channel);
            await RespondAsync(embed: result);
        }

        [SlashCommand("standings_channel_id", "For Admins to set the dynamic standings message.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetStandingsChannelIdAsync(
            [Summary("division", "Which division channel to set.")] string division,
            [Summary("channel", "The text channel to set to.")] IMessageChannel channel)
        {
            var result = _ladderManager.SetStandingsChannelIdProcess(division, channel);
            await RespondAsync(embed: result);
        }

        [SlashCommand("teams_channel_id", "For Admins to set the dynamic teams message.")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetTeamsChannelIdAsync(
            [Summary("division", "Which division channel to set.")] string division,
            [Summary("channel", "The text channel to set to.")] IMessageChannel channel)
        {
            var result = _ladderManager.SetTeamsChannelIdProcess(division, channel);
            await RespondAsync(embed: result);
        }
    }
}
