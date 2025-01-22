using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord.WebSocket;
using Ladderbot4.Managers;

namespace Ladderbot4.Commands
{
    [Group("member", "Slash commands for member related information.")]
    public class MemberSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public MemberSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("mystats", "Sends the user their total stats.")]
        public async Task MyStatsAsync()
        {
            try
            {
                var result = _ladderManager.MemberMyStatsProcess(Context);
                await Context.Interaction.RespondAsync(embed: result, ephemeral: true);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.RespondAsync(embed: errorResult, ephemeral: true);
            }
        }
    }
}
