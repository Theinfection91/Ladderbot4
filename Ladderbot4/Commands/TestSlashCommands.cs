//using Discord;
//using Discord.Interactions;
//using Ladderbot4.Managers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Ladderbot4.Commands
//{
//    [Group("test", "Slash commands related to testing purposes")]
//    public class TestSlashCommands : InteractionModuleBase<SocketInteractionContext>
//    {
//        private readonly LadderManager _ladderManager;

//        public TestSlashCommands(LadderManager ladderManager)
//        {
//            _ladderManager = ladderManager;
//        }

//        [SlashCommand("teams", "Fast team testing")]
//        [Discord.Commands.RequireUserPermission(Discord.GuildPermission.Administrator)]
//        public async Task TestTeamsAsync()
//        {
//            await Context.Interaction.DeferAsync(); // Defer the interaction for async processing

//            // Military alphabet for team names
//            string[] teamNames = { "Alpha", "Bravo", "Charlie", "Delta", "Echo" };
//            string leagueName = "Test1337";

//            // Embed builder for a single consolidated response
//            var embedBuilder = new EmbedBuilder()
//                .WithTitle("🏆 Team Registration Results")
//                .WithColor(Color.Green);

//            foreach (var teamName in teamNames)
//            {
//                try
//                {
//                    // Create a list with the invoking user as a placeholder team member
//                    List<IUser> members = new List<IUser> { Context.User };

//                    // Register the team
//                    var embed = _ladderManager.RegisterTeamToLeagueProcess(Context, teamName, leagueName, members);

//                    // Add the result of each team registration as a field in the embed
//                    embedBuilder.AddField($"Team: {teamName}", embed.Description, inline: false);
//                }
//                catch (Exception ex)
//                {
//                    // Handle any errors during registration and add them to the embed
//                    embedBuilder.AddField($"Team: {teamName}", $"❌ Registration failed: {ex.Message}", inline: false);
//                }
//            }

//            // Send the consolidated embed
//            await Context.Interaction.FollowupAsync(embed: embedBuilder.Build());
//        }

//    }
//}
