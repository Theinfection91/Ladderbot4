﻿using Discord.Interactions;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    [Group("report", "Slash commands related to report data, like report win")]
    public class ReportSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public ReportSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("win", "User-level command to report who won in a match.")]
        public async Task ReportWinAsync(string winningTeamName)
        {
            var result = _ladderManager.ReportWinProcess(Context, winningTeamName);
            await RespondAsync(embed: result);
        }
    }
}
