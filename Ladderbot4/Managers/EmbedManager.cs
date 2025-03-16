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

        #region Debug/Test Embed
        public Embed CreateDebugEmbed(string message)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Debug Embed")
                .WithColor(Color.DarkTeal)
                .AddField("Debug Message", message, inline: false);

            embedBuilder.WithFooter("*").WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        #endregion

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
                .WithDescription($"The ladder for **{leagueRef.Name}** ({leagueRef.Format} League) has been successfully started.")
                .AddField("Format", leagueRef.Format, inline: true)
                .WithFooter("Good luck to all teams!")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed StartLadderAlreadyRunningEmbed(League leagueRef)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Ladder Already Running")
                .WithColor(Color.Red)
                .WithDescription($"The ladder for **{leagueRef.Name}** ({leagueRef.Format} League) is already running.")
                .AddField("Format", leagueRef.Format, inline: true)
                .WithFooter("No changes were made.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed EndLadderSuccessEmbed(League leagueRef)
        {
            // Get the top 3 teams
            Team? firstPlace = leagueRef.Teams.Count > 0 ? leagueRef.Teams[0] : null;
            Team? secondPlace = leagueRef.Teams.Count > 1 ? leagueRef.Teams[1] : null;
            Team? thirdPlace = leagueRef.Teams.Count > 2 ? leagueRef.Teams[2] : null;

            // Prepare the members for each team if they exist
            string firstPlaceMembers = firstPlace?.GetAllMemberNamesToStr() ?? "No members available";
            string secondPlaceMembers = secondPlace?.GetAllMemberNamesToStr() ?? "No members available";
            string thirdPlaceMembers = thirdPlace?.GetAllMemberNamesToStr() ?? "No members available";

            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏁 Ladder Ended")
                .WithColor(Color.Gold)
                .WithDescription($"The ladder for **{leagueRef.Name}** ({leagueRef.Format} League) has officially ended.");

            // 1st Place (Winner) - Always included if there is at least one team
            if (firstPlace != null)
            {
                embedBuilder.AddField("🏆 1st Place - Winner", $"{firstPlace.Name}\n" +
                                                                   $"**Wins**: {firstPlace.Wins} | **Losses**: {firstPlace.Losses}\n" +
                                                                   $"**W/L%**: {firstPlace.WinRatio:P1}\n" +
                                                                   $"**Members**: {firstPlaceMembers}", inline: false);
            }

            // 2nd Place (Runner-up) - Only included if there is at least two teams
            if (secondPlace != null)
            {
                embedBuilder.AddField("🥈 2nd Place - Runner-up", $"{secondPlace.Name}\n" +
                                                                    $"**Wins**: {secondPlace.Wins} | **Losses**: {secondPlace.Losses}\n" +
                                                                    $"**W/L%**: {secondPlace.WinRatio:P1}\n" +
                                                                    $"**Members**: {secondPlaceMembers}", inline: false);
            }

            // 3rd Place - Only included if there is at least three teams
            if (thirdPlace != null)
            {
                embedBuilder.AddField("🥉 3rd Place", $"{thirdPlace.Name}\n" +
                                                     $"**Wins**: {thirdPlace.Wins} | **Losses**: {thirdPlace.Losses}\n" +
                                                     $"**W/L%**: {thirdPlace.WinRatio:P1}\n" +
                                                     $"**Members**: {thirdPlaceMembers}", inline: false);
            }

            // Show all remaining teams in the league
            var remainingTeams = leagueRef.Teams.Skip(3).ToList(); // Skip top 3 teams
            if (remainingTeams.Any())
            {
                // Create a simple string that lists the remaining teams with basic stats
                string remainingTeamsStr = string.Join("\n", remainingTeams.Select(t =>
                    $"{t.Name} - Wins: {t.Wins} | Losses: {t.Losses} | W/L%: {t.WinRatio:P1}"));

                embedBuilder.AddField("Remaining Team(s)", remainingTeamsStr, inline: false);
            }

            // Footer and timestamp
            embedBuilder.WithFooter($"Thanks for participating in the {leagueRef.Name} Ladder!")
                        .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }


        public Embed EndLadderNotRunningEmbed(League leagueRef)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Ladder Not Running")
                .WithColor(Color.Red)
                .WithDescription($"The ladder for **{leagueRef.Name}** ({leagueRef.Format} League) is not currently running.")
                .AddField("Format", leagueRef.Format, inline: true)
                .WithFooter("No changes were made.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed LadderModalErrorEmbed(string errorMessage, string category)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle($"⚠️ {category} Ladder Confirmation Error")
            .WithColor(Color.Red)
            .WithDescription(errorMessage)
            .WithFooter("Please try again.")
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
            .WithDescription($"A new {league.Format} League has been created!")
            .AddField("League Name", $"**{league.Name}**", inline: true)
            .AddField("Format", $"**{league.Format}**", inline: true)
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
            .WithDescription($"A {league.Format} League was deleted!")
            .AddField("League Name", $"**{league.Name}**", inline: true)
            .AddField("Format", $"**{league.Format}**", inline: true)
            .WithFooter("Create a new league or continue an existing one!")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed LeagueModalErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle($"⚠️ Delete League Confirmation Error")
            .WithColor(Color.Red)
            .WithDescription(errorMessage)
            .WithFooter("Please try again.")
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
            .WithDescription($"A new team has been registered to **{league.Name}** ({league.Format} League)")
            .AddField("Team Name", $"**{newTeam.Name}**", inline: true)
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
                .WithDescription($"The team **{team.Name}** has been successfully removed from **{league.Name}** ({league.Format} League).")
                .AddField("Format", league.Format, inline: true)
                .AddField("Removed Members", team.GetAllMemberNamesToStr(), inline: false)
                .WithFooter("Team removal is complete.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed TeamModalErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle($"⚠️ Remove Team Confirmation Error")
            .WithColor(Color.Red)
            .WithDescription(errorMessage)
            .WithFooter("Please try again.")
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
                .WithDescription($"A new challenge has been initiated in **{challengerTeam.League}** ({challengerTeam.LeagueFormat} League)")
                .AddField("Challenger Team", $"{challengerTeam.Name} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("Challenged Team", $"{challengedTeam.Name} (Rank #{challengedTeam.Rank})", inline: true)
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
                .WithTitle("🚫 Challenge Canceled")
                .WithColor(Color.Green)
                .WithDescription($"The challenge sent by **{challengerTeam.Name}** in **{challengerTeam.League}** ({challengerTeam.LeagueFormat} League) has been successfully canceled by a team member.")
                .AddField("Team", $"{challengerTeam.Name} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("League", challengerTeam.League, inline: true)
                .WithFooter("Challenge canceled successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AdminChallengeSuccessEmbed(SocketInteractionContext context, Team challengerTeam, Team challengedTeam)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚔️ Admin-Initiated Challenge!")
                .WithColor(Color.Green)
                .WithDescription($"An admin, **{context.User.GlobalName ?? context.User.Username}**, has initiated a challenge in **{challengerTeam.League}** ({challengerTeam.LeagueFormat} League).")
                .AddField("Challenger Team", $"{challengerTeam.Name} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("Challenged Team", $"{challengedTeam.Name} (Rank #{challengedTeam.Rank})", inline: true)
                .WithFooter("Challenge initiated by an Admin")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AdminCancelChallengeSuccessEmbed(SocketInteractionContext context, Team challengerTeam, Team challengedTeam)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🚫 Challenge Canceled by Admin")
                .WithColor(Color.Green)
                .WithDescription($"The challenge sent by **{challengerTeam.Name}** in **{challengerTeam.League}** ({challengerTeam.LeagueFormat} League) against **{challengedTeam.Name}** has been successfully canceled by an admin.")
                .AddField("Admin", context.User.GlobalName ?? context.User.Username, inline: true)
                .AddField("Team", $"{challengerTeam.Name} (Rank #{challengerTeam.Rank})", inline: true)
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
                    ? $"Team **{winningTeam.Name}** has won the challenge they initiated against **{losingTeam.Name}** in **{league.Name}** ({league.Format} League) and taken their rank of **#{winningTeam.Rank}**! Team **{losingTeam.Name}** drops down to **#{losingTeam.Rank}**. All other ranks have been adjusted accordingly."
                    : $"Team **{winningTeam.Name}** has defeated **{losingTeam.Name}** in **{league.Name}** ({league.Format} League) and defended their rank. No rank changes occurred.")
                .AddField("Winning Team", $"{winningTeam.Name} (Rank #{winningTeam.Rank})", inline: true)
                .AddField("Losing Team", $"{losingTeam.Name} (Rank #{losingTeam.Rank})", inline: true)
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
                    ? $"Team **{winningTeam.Name}** has won the challenge they initiated against **{losingTeam.Name}** in **{league.Name}** ({league.Format} League) and taken their rank of **#{winningTeam.Rank}**! Team **{losingTeam.Name}** drops down to **#{losingTeam.Rank}**. All other ranks have been adjusted accordingly. This report was created by an Admin (**{context.User.GlobalName ?? context.User.Username}**) "
                    : $"Team **{winningTeam.Name}** has defeated **{losingTeam.Name}** in **{league.Name}** ({league.Format} League) and defended their rank. No rank changes occurred.")
                .AddField("Winning Team", $"{winningTeam.Name} (Rank #{winningTeam.Rank})", inline: true)
                .AddField("Losing Team", $"{losingTeam.Name} (Rank #{losingTeam.Rank})", inline: true)
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
                .WithTitle($"🏆 Standings - **{league.Name}** ({league.Format} League)")
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
                        $"#{team.Rank} {team.Name}",
                        $"**Wins:** {team.Wins} | **Losses:** {team.Losses}\n" +
                        $"**Win Streak:** {team.WinStreak} | **Loss Streak:** {team.LoseStreak}\n" +
                        $"**Win Ratio:** {winRatio} | **Challenge Status:** {status}",
                        inline: false // Stacked vertically for better readability
                    );
                }
            }
            else
            {
                embedBuilder.WithDescription($"🔎 No teams are currently registered in **{league.Name}** ({league.Format} League).");
            }

            // Add a footer with timestamp
            embedBuilder.WithFooter("Last Updated")
                        .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed PostLeaguesEmbed(List<League> leagues)
        {
            // Create the embed
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"📋 Active Leagues")
                .WithColor(Color.Purple)
                .WithDescription($"Overview of all leagues:")
                .WithFooter("Last Updated")
                .WithTimestamp(DateTimeOffset.Now);

            if (leagues.Count > 0)
            {
                foreach (var league in leagues)
                {
                    // Check if league has teams
                    if (league.Teams.Count > 0)
                    {
                        // Format the teams by rank
                        var teamList = string.Join("\n", league.Teams
                            .OrderBy(t => t.Rank)
                            .Select(t => $"#{t.Rank} {t.Name}"));

                        embedBuilder.AddField(
                            $"🏆 **{league.Name}** ({league.Format} League)",
                            $"**Teams by Rank:**\n{teamList}",
                            inline: false // Stacked vertically for better readability
                        );
                    }
                    else
                    {
                        embedBuilder.AddField(
                            $"🏆 **{league.Name}** ({league.Format} League)",
                            "🔎 No teams are currently registered.",
                            inline: false
                        );
                    }
                }
            }
            else
            {
                embedBuilder.WithDescription("🔎 No active leagues available at this time.");
            }

            return embedBuilder.Build();
        }

        public Embed PostChallengesEmbed(League league, List<Challenge> challenges)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"⚔️ Active Challenges - **{league.Name}** ({league.Format} League)")
                .WithColor(Color.Orange)
                .WithDescription($"Current challenges for league:");

            if (challenges.Count > 0)
            {
                foreach (var challenge in challenges)
                {
                    embedBuilder.AddField(
                        $"{challenge.Challenger} (#{challenge.ChallengerRank}) 🆚 {challenge.Challenged} (#{challenge.ChallengedRank})",
                        $"**Created On:** {challenge.CreatedOn:MM/dd/yyyy HH:mm}",
                        inline: false
                    );
                }
            }
            else
            {
                embedBuilder.WithDescription($"🔎 No active challenges in **{league.Name}** ({league.Format} League) at this time.");
            }

            embedBuilder.WithFooter("Last Updated").WithTimestamp(DateTimeOffset.Now);
            return embedBuilder.Build();
        }

        public Embed PostTeamsEmbed(League league, List<Team> teams)
        {
            // Create the embed builder
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"🛡️ Teams - **{league.Name}** ({league.Format} League)")
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
                        $"{team.Name} (#{team.Rank})",
                        $"**Members:** {team.GetAllMemberNamesToStr()}\n" +
                        $"**Challenge Status:** {challengeStatus}",
                        inline: false // Stacked vertically for readability
                    );
                }
            }
            else
            {
                embedBuilder.WithDescription($"🔎 No teams in **{league.Name}** ({league.Format} League) at this time.");
            }

            // Build and return the embed
            return embedBuilder.Build();
        }

        public Embed PostLeaguesErrorEmbed(string divisionType)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Invalid Division Type")
                .WithColor(Color.Red)
                .WithDescription($"You have entered an invalid division type: {divisionType}\nEnter '1v1', '2v2', '3v3' or do not add an argument to post all leagues. Please try again.")
                .WithFooter("Post Leagues failed.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        #endregion

        #region Set Rank
        public Embed SetRankErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("⚠️ Set Rank Error")
            .WithColor(Color.Red)
            .WithDescription(errorMessage)
            .WithFooter("Please try again.")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed SetRankSuccessEmbed(Team team, League league)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Set Rank Success")
                .WithColor(Color.Green)
                .WithDescription($"Team {team.Name} has been moved to rank {team.Rank} in **{league.Name}** ({league.Format} League). All ranks have been adjusted accordingly.")
                .WithFooter("Team rank successfully changed.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        #endregion

        #region Add/Subtract Win/Loss
        public Embed AddToWinCountSuccessEmbed(Team team, int numberOfWins, SocketInteractionContext context)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Wins Added Successfully!")
                .WithColor(Color.Green)
                .WithDescription($"**{numberOfWins}** win(s) have been added to **{team.Name}**'s win count by Admin **{context.User.GlobalName ?? context.User.Username}**.")
                .AddField("Team", team.Name, inline: true)
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
                .WithDescription($"**{numberOfWins}** win(s) have been subtracted from **{team.Name}**'s win count by Admin **{context.User.GlobalName ?? context.User.Username}**.")
                .AddField("Team", team.Name, inline: true)
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
                .WithDescription($"**{numberOfLosses}** loss(es) have been added to **{team.Name}**'s loss count by Admin **{context.User.GlobalName ?? context.User.Username}**.")
                .AddField("Team", team.Name, inline: true)
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
                .WithDescription($"**{numberOfLosses}** loss(es) have been subtracted from **{team.Name}**'s loss count by Admin **{context.User.GlobalName ?? context.User.Username}**.")
                .AddField("Team", team.Name, inline: true)
                .AddField("New Loss Count", team.Losses.ToString(), inline: true)
                .WithFooter("Loss count updated successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Add Member
        public Embed AddMemberSuccessEmbed(Team team)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("👤 Add Member Success!")
            .WithColor(Color.Green)
            .WithDescription($"Member(s) was successfully added to team.")
            .AddField("Team Name", $"**{team.Name}**", inline: true)
            .AddField("Members", team.GetAllMemberNamesToStr(), inline: false)
            .WithFooter("Good luck to your team!")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AddMemberErrorEmbed(string message)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Add Member Error")
                .WithColor(Color.Red)
                .WithDescription(message)
                .WithFooter("Add Member failed.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion

        #region Member Stats
        public Embed MemberMyStatsEmbed(MemberProfile memberProfile)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle($"📊 My Member Stats: {memberProfile.DisplayName}")
                .WithColor(Color.Teal)

                // New fields for Title, Level, XP, and XP to next level (placed above the stats)
                .AddField("🏆 Title", memberProfile.Title, inline: true)
                .AddField("💼 Level", memberProfile.Level.ToString(), inline: true)
                .AddField("💥 Total XP", $"{memberProfile.Experience} XP", inline: true)
                .AddField("⏳ XP to Next Level", $"{memberProfile.ExperienceToNextLevel} XP", inline: true)

                // Old stats fields
                .AddField("🏅 Wins", memberProfile.Wins.ToString(), inline: true)
                .AddField("❌ Losses", memberProfile.Losses.ToString(), inline: true)
                .AddField("🎖️ League Championships", memberProfile.LeagueChampionships.ToString(), inline: true)
                .AddField("📊 Matches Played", memberProfile.TotalMatchCount.ToString(), inline: true)
                .AddField("🎗️ Seasons Completed", memberProfile.TotalSeasons.ToString(), inline: true)
                .AddField("📈 Win/Loss Ratio", $"{(memberProfile.WinLossRatio * 100):0.00}%", inline: true)

                .WithFooter("Check back as your stats update as you play!")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed MemberMyStatsErrorEmbed(string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("⚠️ My Member Stats Error")
            .WithColor(Color.Red)
            .WithDescription(errorMessage)
            .WithFooter("Please try again.")
            .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed MemberLeaderboardEmbed(List<MemberProfile> memberProfiles)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("📊 Member Leaderboard")
                .WithColor(Color.Blue)
                .WithFooter($"Total Members: {memberProfiles.Count}")
                .WithTimestamp(DateTimeOffset.Now);

            if (memberProfiles == null || !memberProfiles.Any())
            {
                embedBuilder.WithDescription("No members found to display on the leaderboard.");
                return embedBuilder.Build();
            }

            // Sort members by Wins, then by WinLossRatio (descending order)
            var sortedMembers = memberProfiles
                .OrderByDescending(m => m.Wins)
                .ThenByDescending(m => m.WinLossRatio)
                .ToList();

            foreach (var member in sortedMembers)
            {
                string stats = $"**Title**: {member.Title} | **Level**: {member.Level}\n" +
                               $"**Wins**: {member.Wins} | **Losses**: {member.Losses} | " +
                               $"**W/L Ratio**: {(member.WinLossRatio * 100):F2}%\n" +
                               $"**Matches Played**: {member.TotalMatchCount} | **Seasons Completed**: {member.TotalSeasons}\n" +
                               $"**League Championships**: {member.LeagueChampionships}";

                embedBuilder.AddField(member.DisplayName, stats, inline: false);

                // Limit the embed to 25 fields (Discord API restriction)
                if (embedBuilder.Fields.Count >= 25)
                    break;
            }

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
                .WithDescription($"**{leagueName}** was not found in the League database. Please try again.")
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
                .WithDescription($"Attempting to subtract **{attemptedChange}** {statType.ToLower()}(s) from **{team.Name}**'s current {statType.ToLower()} count of **{(statType == "Wins" ? team.Wins : team.Losses)}** would result in a negative number. Operation aborted.")
                .AddField("Team", team.Name, inline: true)
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
        public Embed SetChannelIdSuccessEmbed(League league, IMessageChannel channel, string type)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Channel Set Successfully")
                .WithColor(Color.Green)
                .WithDescription($"The {type} channel for **{league.Name}** ({league.Format} League) has been successfully set.")
                .AddField("Channel Name", channel.Name, inline: true)
                .AddField("Channel ID", channel.Id.ToString(), inline: true)
                .AddField("League", league.Name, inline: true)
                .WithFooter("Channel configuration updated")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed SetChannelIdErrorEmbed(League league, IMessageChannel channel, string type, string errorMessage)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Channel Set Error")
                .WithColor(Color.Red)
                .WithDescription($"Failed to set the {type} channel for **{league.Name}** ({league.Format} League).")
                .AddField("Error", errorMessage, inline: false)
                .AddField("Attempted Channel ID", channel.Id.ToString(), inline: true)
                .AddField("League", league.Name, inline: true)
                .WithFooter("Channel configuration failed")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }
        #endregion
    }
}
