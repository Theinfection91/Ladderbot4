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
    public class SettingsTestingCommands : ModuleBase<SocketCommandContext>
    {
        private readonly LadderManager _ladderManager;

        public SettingsTestingCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [Command("set_guild_id", Aliases = ["sgid"])]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SetGuildIdAsync()
        {
            string result = _ladderManager.SetGuildId(Context);
            await ReplyAsync(result);
        }

        // Method for turning Super Admin on or off. For now its just a property in LadderManager. Will migrate and finish implementing in Settings later.
        [Command("super_admin", Aliases = ["sa"])]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        // Super Admin Mode to Bypass certain requirements for solo testing
        public async Task SuperAdminModeAsync(string trueOrFalse)
        {
            string result = _ladderManager.SetSuperAdminMode(trueOrFalse);
            await ReplyAsync(result);
        }
    }
}
