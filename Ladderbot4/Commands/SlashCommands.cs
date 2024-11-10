using Discord.Interactions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public SlashCommands() { }

        [SlashCommand("ping", "Replies with pong.")]
        public async Task PingCommand()
        {
            await RespondAsync("Pong!");
        }
    }
}
