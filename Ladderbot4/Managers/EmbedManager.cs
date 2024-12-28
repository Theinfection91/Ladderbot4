using Discord;
using Discord.Interactions;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class EmbedManager
    {
        public EmbedManager()
        {

        }

        #region Try-Catch Error
        public Embed CreateErrorEmbed(Exception ex, string commandName)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Error Occurred")
                .WithColor(Color.Red)
                .WithDescription("An unexpected error has occurred while processing your request.")
                .AddField("Command Class", commandName, inline: true)
                .AddField("Error Message", ex.Message, inline: false);

            if (ex.StackTrace != null)
            {
                embedBuilder.AddField("Stack Trace", $"```{ex.StackTrace}```", inline: false);
            }

            embedBuilder.WithFooter("Please report this issue to the admin.").WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        #endregion

        #region Start/End Ladder
        public Embed StartLadderSuccessEmbed(League leagueRef)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏁 Ladder Started!")
                .WithColor(Color.Green)
                .WithDescription($"The ladder for **{leagueRef.LeagueName}** ({leagueRef.Division} League) has been successfully started.")
                .AddField("Division Type", leagueRef.Division, inline: true)
                .WithFooter("Good luck to all teams!")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed StartLadderAlreadyRunningEmbed(League leagueRef)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Ladder Already Running")
                .WithColor(Color.Red)
                .WithDescription($"The ladder for **{leagueRef.LeagueName}** ({leagueRef.Division} League) is already running.")
                .AddField("Division Type", leagueRef.Division, inline: true)
                .WithFooter("No changes were made.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed EndLadderSuccessEmbed(League leagueRef)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏁 Ladder Ended")
                .WithColor(Color.Green)
                .WithDescription($"The ladder for **{leagueRef.LeagueName}** ({leagueRef.Division} League) has successfully ended.")
                .AddField("Division Type", leagueRef.Division, inline: true)
                .WithFooter("Thank you for participating!")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed EndLadderNotRunningEmbed(League leagueRef)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Ladder Not Running")
                .WithColor(Color.Red)
                .WithDescription($"The ladder for **{leagueRef.LeagueName}** ({leagueRef.Division} League) is not currently running.")
                .AddField("Division Type", leagueRef.Division, inline: true)
                .WithFooter("No changes were made.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Create/Delete League
        public Embed CreateLeagueErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("⚠️ League Creation Error")
            .WithColor(Color.Red)
            .WithDescription(errorMessage)
            .WithFooter("Please try again.")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed CreateLeagueSuccessEmbed(League league)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("🏅 League Created Successfully!")
            .WithColor(Color.Green)
            .WithDescription($"A new {league.Division} League has been created!")
            .AddField("League Name", $"**{league.LeagueName}**", inline: true)
            .AddField("Division", $"**{league.Division}**", inline: true)
            .WithFooter("Let's add some teams and get started!")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed DeleteLeagueErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("⚠️ Delete League Error")
            .WithColor(Color.Red)
            .WithDescription(errorMessage)
            .WithFooter("Please try again.")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed DeleteLeagueSuccessEmbed(League league)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("✅ League Deleted Successfully!")
            .WithColor(Color.Green)
            .WithDescription($"A {league.Division} League was deleted!")
            .AddField("League Name", $"**{league.LeagueName}**", inline: true)
            .AddField("Division", $"**{league.Division}**", inline: true)
            .WithFooter("Create a new league or continue an existing one!")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Register/Remove Team
        public Embed RegisterTeamErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("⚠️ Registration Error")
            .WithColor(Color.Red)
            .WithDescription(errorMessage)
            .WithFooter("Please try again.")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed RegisterTeamToLeagueSuccessEmbed(Team newTeam, League league)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("🎉 Team Registered Successfully!")
            .WithColor(Color.Green)
            .WithDescription($"A new team has been registered to **{league.LeagueName}** ({league.Division} League)")
            .AddField("Team Name", $"**{newTeam.TeamName}**", inline: true)
            .AddField("Rank", $"**#{newTeam.Rank}**", inline: true)
            .AddField("Members", newTeam.GetAllMemberNamesToStr(), inline: false)
            .WithFooter("Good luck to your team!")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed RemoveTeamErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Team Removal Error")
                .WithColor(Color.Red)
                .WithDescription(errorMessage)
                .WithFooter("Please verify the team name and try again.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed RemoveTeamSuccessEmbed(Team team, League league)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Team Removed Successfully")
                .WithColor(Color.Green)
                .WithDescription($"The team **{team.TeamName}** has been successfully removed from **{league.LeagueName}** ({league.Division} League).")
                .AddField("Division", league.Division, inline: true)
                .AddField("Removed Members", team.GetAllMemberNamesToStr(), inline: false)
                .WithFooter("Team removal is complete.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Challenge Send/Cancel
        public Embed ChallengeErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Challenge Error")
                .WithColor(Color.Red)
                .WithDescription(errorMessage)
                .WithFooter("Please resolve the issue and try again.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed ChallengeSuccessEmbed(Team challengerTeam, Team challengedTeam, Challenge challenge)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚔️ Challenge Initiated!")
                .WithColor(Color.Green)
                .WithDescription($"A new challenge has been initiated in **{challengerTeam.League}** ({challengerTeam.Division} League)")
                .AddField("Challenger Team", $"{challengerTeam.TeamName} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("Challenged Team", $"{challengedTeam.TeamName} (Rank #{challengedTeam.Rank})", inline: true)
                .WithFooter("Best of luck to both teams!")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed CancelChallengeErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Cancel Challenge Error")
                .WithColor(Color.Red)
                .WithDescription(errorMessage)
                .WithFooter("Please resolve the issue and try again.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed CancelChallengeSuccessEmbed(Team challengerTeam)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Challenge Canceled")
                .WithColor(Color.Green)
                .WithDescription($"The challenge sent by **{challengerTeam.TeamName}** in **{challengerTeam.League}** ({challengerTeam.Division} League) has been successfully canceled.")
                .AddField("Team", $"{challengerTeam.TeamName} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("League", challengerTeam.League, inline: true)
                .WithFooter("Challenge canceled successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AdminChallengeSuccessEmbed(Team challengerTeam, Team challengedTeam, SocketInteractionContext context)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚔️ Admin-Initiated Challenge!")
                .WithColor(Color.Green)
                .WithDescription($"An admin, **{context.User.GlobalName ?? context.User.Username}**, has initiated a challenge in **{challengerTeam.League}** ({challengerTeam.Division} League).")
                .AddField("Challenger Team", $"{challengerTeam.TeamName} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("Challenged Team", $"{challengedTeam.TeamName} (Rank #{challengedTeam.Rank})", inline: true)
                .WithFooter("Challenge initiated by an Admin")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AdminCancelChallengeSuccessEmbed(Team challengerTeam, SocketInteractionContext context)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🚫 Challenge Canceled by Admin")
                .WithColor(Color.Green)
                .WithDescription($"The challenge sent by **{challengerTeam.TeamName}** in **{challengerTeam.League}** ({challengerTeam.Division} League) has been successfully canceled by an admin.")
                .AddField("Admin", context.User.GlobalName ?? context.User.Username, inline: true)
                .AddField("Team", $"{challengerTeam.TeamName} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("League", challengerTeam.League, inline: true)
                .WithFooter("Challenge canceled successfully by Admin")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Report Win
        public Embed ReportWinErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Match Reporting Error")
                .WithColor(Color.Red)
                .WithDescription(errorMessage)
                .WithFooter("Please resolve the issue and try again.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed ReportWinSuccessEmbed(Team winningTeam, Team losingTeam, bool rankChange, League league)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏆 Match Result Reported!")
                .WithColor(Color.Green)
                .WithDescription(rankChange
                    ? $"Team **{winningTeam.TeamName}** has won the challenge they initiated against **{losingTeam.TeamName}** in **{league.LeagueName}** ({league.Division} League) and taken their rank of **#{winningTeam.Rank}**! Team **{losingTeam.TeamName}** drops down to **#{losingTeam.Rank}**. All other ranks have been adjusted accordingly."
                    : $"Team **{winningTeam.TeamName}** has defeated **{losingTeam.TeamName}** in **{league.LeagueName}** ({league.Division} League) and defended their rank. No rank changes occurred.")
                .AddField("Winning Team", $"{winningTeam.TeamName} (Rank #{winningTeam.Rank})", inline: true)
                .AddField("Losing Team", $"{losingTeam.TeamName} (Rank #{losingTeam.Rank})", inline: true)
                .WithFooter("Match successfully reported")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed ReportWinAdminSuccessEmbed(SocketInteractionContext context, Team winningTeam, Team losingTeam, bool rankChange, League league)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏆 Match Result Reported By Admin!")
                .WithColor(Color.Green)
                .WithDescription(rankChange
                    ? $"Team **{winningTeam.TeamName}** has won the challenge they initiated against **{losingTeam.TeamName}** in **{league.LeagueName}** ({league.Division} League) and taken their rank of **#{winningTeam.Rank}**! Team **{losingTeam.TeamName}** drops down to **#{losingTeam.Rank}**. All other ranks have been adjusted accordingly. This report was created by an Admin (**{context.User.GlobalName ?? context.User.Username}**) "
                    : $"Team **{winningTeam.TeamName}** has defeated **{losingTeam.TeamName}** in **{league.LeagueName}** ({league.Division} League) and defended their rank. No rank changes occurred.")
                .AddField("Winning Team", $"{winningTeam.TeamName} (Rank #{winningTeam.Rank})", inline: true)
                .AddField("Losing Team", $"{losingTeam.TeamName} (Rank #{losingTeam.Rank})", inline: true)
                .WithFooter("Match successfully reported by Admin")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        #endregion

        #region Post Standings/Challenges/Teams
        public Embed PostStandingsEmbed(League league)
        {
            // Create the embed
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"🏆 Standings - **{league.LeagueName}** ({league.Division} League)")
                .WithColor(Color.Gold)
                .WithDescription("Current standings for league:");

            if (league.Teams.Count > 0)
            {
                // Format the standings
                foreach (Team team in league.Teams)
                {
                    string status = team.IsChallengeable ? "Free" : "Challenged";
                    string winRatio = $"{team.WinRatio:P1}"; // Formats as percentage with 1 decimal place
                    embedBuilder.AddField(
                        $"#{team.Rank} {team.TeamName}",
                        $"**Wins:** {team.Wins} | **Losses:** {team.Losses}\n" +
                        $"**Win Streak:** {team.WinStreak} | **Loss Streak:** {team.LoseStreak}\n" +
                        $"**Win Ratio:** {winRatio} | **Challenge Status:** {status}",
                        inline: false // Stacked vertically for better readability
                    );
                }
            }
            else
            {
                embedBuilder.WithDescription($"🔎 No teams are currently registered in **{league.LeagueName}** ({league.Division} League).");
            }

            // Add a footer with timestamp
            embedBuilder.WithFooter("Updated")
                        .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }


        public Embed PostChallengesEmbed(League league, List<Challenge> challenges)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"⚔️ Active Challenges - **{league.LeagueName}** ({league.Division} League)")
                .WithColor(Color.Orange)
                .WithDescription($"Current challenges for league:");

            if (challenges.Count > 0)
            {
                foreach (var challenge in challenges)
                {
                    embedBuilder.AddField(
                        $"{challenge.Challenger} (Rank {challenge.ChallengerRank}) 🆚 {challenge.Challenged} (Rank {challenge.ChallengedRank})",
                        $"**Created On:** {challenge.CreatedOn:MM/dd/yyyy HH:mm}",
                        inline: false
                    );
                }
            }
            else
            {
                embedBuilder.WithDescription($"🔎 No active challenges in **{league.LeagueName}** ({league.Division} League) at this time.");
            }

            embedBuilder.WithFooter("Last Updated").WithTimestamp(DateTimeOffset.Now);
            return embedBuilder.Build();
        }

        public Embed PostTeamsEmbed(League league, List<Team> teams)
        {
            // Create the embed builder
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"🛡️ Teams - **{league.LeagueName}** ({league.Division} League)")
                .WithColor(Color.Blue)
                .WithFooter("Last Updated")
                .WithTimestamp(DateTimeOffset.Now);

            // Check if there are any teams in the list
            if (teams.Count > 0)
            {
                embedBuilder.WithDescription($"Current teams in league by rank:");

                // Add a field for each team
                foreach (Team team in teams)
                {
                    string challengeStatus = team.IsChallengeable ? "Free" : "Challenged";

                    embedBuilder.AddField(
                        $"{team.TeamName} (#{team.Rank})",
                        $"**Members:** {team.GetAllMemberNamesToStr()}\n" +
                        $"**Challenge Status:** {challengeStatus}",
                        inline: false // Stacked vertically for readability
                    );
                }
            }
            else
            {
                embedBuilder.WithDescription($"🔎 No teams in **{league.LeagueName}** ({league.Division} League) at this time.");
            }

            // Build and return the embed
            return embedBuilder.Build();
        }

        #endregion

        #region Add/Subtract Win/Loss
        public Embed AddToWinCountSuccessEmbed(Team team, int numberOfWins, SocketInteractionContext context)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Wins Added Successfully!")
                .WithColor(Color.Green)
                .WithDescription($"**{numberOfWins}** win(s) have been added to **{team.TeamName}**'s win count by Admin **{context.User.GlobalName ?? context.User.Username}**.")
                .AddField("Team", team.TeamName, inline: true)
                .AddField("New Win Count", team.Wins.ToString(), inline: true)
                .WithFooter("Win count updated successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed SubtractFromWinCountSuccessEmbed(Team team, int numberOfWins, SocketInteractionContext context)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Wins Subtracted Successfully!")
                .WithColor(Color.Green)
                .WithDescription($"**{numberOfWins}** win(s) have been subtracted from **{team.TeamName}**'s win count by Admin **{context.User.GlobalName ?? context.User.Username}**.")
                .AddField("Team", team.TeamName, inline: true)
                .AddField("New Win Count", team.Wins.ToString(), inline: true)
                .WithFooter("Win count updated successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AddToLossCountSuccessEmbed(Team team, int numberOfLosses, SocketInteractionContext context)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Losses Added Successfully!")
                .WithColor(Color.Green)
                .WithDescription($"**{numberOfLosses}** loss(es) have been added to **{team.TeamName}**'s loss count by Admin **{context.User.GlobalName ?? context.User.Username}**.")
                .AddField("Team", team.TeamName, inline: true)
                .AddField("New Loss Count", team.Losses.ToString(), inline: true)
                .WithFooter("Loss count updated successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed SubtractFromLossCountSuccessEmbed(Team team, int numberOfLosses, SocketInteractionContext context)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Losses Subtracted Successfully!")
                .WithColor(Color.Green)
                .WithDescription($"**{numberOfLosses}** loss(es) have been subtracted from **{team.TeamName}**'s loss count by Admin **{context.User.GlobalName ?? context.User.Username}**.")
                .AddField("Team", team.TeamName, inline: true)
                .AddField("New Loss Count", team.Losses.ToString(), inline: true)
                .WithFooter("Loss count updated successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region League/Team Not Found
        public Embed TeamNotFoundErrorEmbed(string teamName)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Team Not Found")
                .WithColor(Color.Red)
                .WithDescription($"The team **{teamName}** was not found in the database. Please try again.")
                .WithFooter("Team name verification failed.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed LeagueNotFoundErrorEmbed(string leagueName)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ League Not Found")
                .WithColor(Color.Red)
                .WithDescription($"The League **{leagueName}** was not found in the database. Please try again.")
                .WithFooter("League name verification failed.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Negative Count
        public Embed NegativeCountErrorEmbed(Team team, int attemptedChange, string statType)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Invalid Operation")
                .WithColor(Color.Red)
                .WithDescription($"Attempting to subtract **{attemptedChange}** {statType.ToLower()}(s) from **{team.TeamName}**'s current {statType.ToLower()} count of **{(statType == "Wins" ? team.Wins : team.Losses)}** would result in a negative number. Operation aborted.")
                .AddField("Team", team.TeamName, inline: true)
                .AddField("Current Count", (statType == "Wins" ? team.Wins : team.Losses).ToString(), inline: true)
                .WithFooter($"{statType} update failed due to invalid count.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Super Admin and Guild ID
        public Embed SetGuildIdSuccessEmbed(ulong guildId)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Guild ID Set Successfully")
                .WithColor(Color.Green)
                .WithDescription($"The Guild ID has been set to **{guildId}** in `config.json`.")
                .WithFooter("If this is the first time setting the Guild ID, please restart the bot.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed SuperAdminModeOnEmbed()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Super Admin Mode Enabled")
                .WithColor(Color.Green)
                .WithDescription("Super Admin Mode is now **ON**.")
                .WithFooter("All Super Admin privileges are active.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed SuperAdminModeOffEmbed()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Super Admin Mode Disabled")
                .WithColor(Color.Green)
                .WithDescription("Super Admin Mode is now **OFF**.")
                .WithFooter("Super Admin privileges have been disabled.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed SuperAdminInvalidInputEmbed()
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Invalid Input")
                .WithColor(Color.Red)
                .WithDescription("The input provided for enabling or disabling Super Admin Mode is invalid. Please use `on` or `off`.")
                .WithFooter("Operation aborted.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AddSuperAdminIdSuccessEmbed(IUser user)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Super Admin Added")
                .WithColor(Color.Green)
                .WithDescription($"**{user.Username}** (ID: **{user.Id}**) has been added to the Super Admin list.")
                .WithFooter("Super Admin added successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed RemoveSuperAdminIdNotFoundEmbed(IUser user)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Super Admin Not Found")
                .WithColor(Color.Red)
                .WithDescription($"**{user.Username}** (ID: **{user.Id}**) is not in the Super Admin list and cannot be removed.")
                .WithFooter("No changes made.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed RemoveSuperAdminIdSuccessEmbed(IUser user)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Super Admin Removed")
                .WithColor(Color.Green)
                .WithDescription($"**{user.Username}** (ID: **{user.Id}**) has been removed from the Super Admin list.")
                .WithFooter("Super Admin removed successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AddSuperAdminIdAlreadyExistsEmbed(IUser user)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Super Admin Already Exists")
                .WithColor(Color.Red)
                .WithDescription($"**{user.Username}** (ID: **{user.Id}**) is already in the Super Admin list and cannot be added again.")
                .WithFooter("No changes made.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Set Channel
        public Embed SetChannelIdSuccessEmbed(string division, IMessageChannel channel, string type)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Channel Set Successfully")
                .WithColor(Color.Green)
                .WithDescription($"The {type} channel for the **{division} Division** has been set successfully.")
                .AddField("Channel Name", channel.Name, inline: true)
                .AddField("Channel ID", channel.Id.ToString(), inline: true)
                .AddField("Division", division, inline: true)
                .WithFooter("Channel configuration updated")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed SetChannelIdErrorEmbed(string division, IMessageChannel channel, string type, string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Channel Set Error")
                .WithColor(Color.Red)
                .WithDescription($"Failed to set the {type} channel for the **{division} Division**.")
                .AddField("Error", errorMessage, inline: false)
                .AddField("Attempted Channel ID", channel.Id.ToString(), inline: true)
                .AddField("Division", division, inline: true)
                .WithFooter("Channel configuration failed")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion
    }
}
