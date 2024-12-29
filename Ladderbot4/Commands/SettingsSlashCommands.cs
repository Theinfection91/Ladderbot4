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
    [Group("settings", "Commands related to changing certain settings in Settings\\config.json")]
    public class SettingsSlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly LadderManager _ladderManager;

        public SettingsSlashCommands(LadderManager ladderManager)
        {
            _ladderManager = ladderManager;
        }

        [SlashCommand("super_admin_on_off", "Sets the super admin mode to on (true) or off (false).")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task SuperAdminOnOffAsync(
            [Summary("onOrOff", "Use on or off to set the Super Admin mode.")] string onOrOff)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.SetSuperAdminModeOnOffProcess(onOrOff.Trim().ToLower());
                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("add_admin_id", "Adds a given user's Id to list of Super Admin Id's")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task AddSuperAdminIdAsync(
            [Summary("user", "The user to be added to the list in config.json")] IUser user)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.AddSuperAdminIdProcess(user);
                await Context.Interaction.FollowupAsync(embed: result);
            }
            catch (Exception ex)
            {
                string commandName = (Context.Interaction as SocketSlashCommand)?.Data.Name ?? "Unknown Command";
                var errorResult = _ladderManager.ExceptionErrorHandlingProcess(ex, commandName);
                await Context.Interaction.FollowupAsync(embed: errorResult);
            }
        }

        [SlashCommand("remove_admin_id", "Deletes a given user's Id from the list of Super Admin Id's")]
        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task RemoveSuperAdminIdAsync(
            [Summary("user", "The user to be removed from the list in config.json")] IUser user)
        {
            try
            {
                await Context.Interaction.DeferAsync();
                var result = _ladderManager.RemoveSuperAdminIdProcess(user);
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
