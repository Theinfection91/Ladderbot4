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
    public class NonSlashCommands : ModuleBase<SocketCommandContext>
    {
        private readonly LadderManager _ladderManager;

        public NonSlashCommands(LadderManager ladderManager)
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
    }
}
