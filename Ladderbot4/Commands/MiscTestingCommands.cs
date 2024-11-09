using Discord.Commands;
using Discord.WebSocket;
using Ladderbot4.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Commands
{
    public class MiscTestingCommands : ModuleBase<SocketCommandContext>
    {
        private LadderManager _ladderManager;

        public MiscTestingCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [Command("super_admin", Aliases = ["sa"])]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        // Super Admin Mode to Bypass certain requirements for easier testing
        public async Task SuperAdminModeAsync(string trueOrFalse)
        {
            string result = _ladderManager.SetSuperAdminMode(trueOrFalse);
            await ReplyAsync(result);
        }
    }
}
