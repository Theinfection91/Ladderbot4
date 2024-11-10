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
    [Group("settings", "Commands related to changing certain settings in Settings\\config.json")]
    public class SettingsSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public SettingsSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("add_admin_id", "Adds a given user's Id to list of Super Admin Id's")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task AddSuperAdminIdAsync(
            [Summary("user", "The user to be added to the list in config.json")] IUser user)
        {
            string result = _ladderManager.AddSuperAdminId(user);
            await RespondAsync(result);
        }

        [SlashCommand("remove_admin_id", "Deletes a given user's Id from the list of Super Admin Id's")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task RemoveSuperAdminIdAsync(
            [Summary("user", "The user to be removed from the list in config.json")] IUser user)
        {
            string result = _ladderManager.RemoveSuperAdminId(user);
            await RespondAsync(result);
        }
    }
}
