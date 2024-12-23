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

        public Embed StartLadderSuccessEmbed(string division)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏁 Ladder Started!")
                .WithColor(Color.Green)
                .WithDescription($"The ladder for the **{division} Division** has been successfully started.")
                .AddField("Division", division, inline: true)
                .WithFooter("Good luck to all teams!")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed StartLadderAlreadyRunningEmbed(string division)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Ladder Already Running")
                .WithColor(Color.Red)
                .WithDescription($"The ladder for the **{division} Division** is already running.")
                .AddField("Division", division, inline: true)
                .WithFooter("No changes were made.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed EndLadderSuccessEmbed(string division)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏁 Ladder Ended")
                .WithColor(Color.Green)
                .WithDescription($"The ladder for the **{division} Division** has been successfully ended.")
                .AddField("Division", division, inline: true)
                .WithFooter("Thank you for participating!")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed EndLadderNotRunningEmbed(string division)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚠️ Ladder Not Running")
                .WithColor(Color.Red)
                .WithDescription($"The ladder for the **{division} Division** hasn't started yet.")
                .AddField("Division", division, inline: true)
                .WithFooter("No changes were made.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

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
            .WithDescription($"A new team has been registered to the following League: **{league.LeagueName}**!")
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
                .WithDescription($"The team **{team.TeamName}** has been successfully removed from the **{league.LeagueName}** League.")
                .AddField("Division", league.Division, inline: true)
                .AddField("Removed Members", team.GetAllMemberNamesToStr(), inline: false)
                .WithFooter("Team removal is complete.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

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
                .WithDescription($"A new challenge has been initiated in the **{challengerTeam.League}** League!")
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
                .WithDescription($"The challenge sent by **{challengerTeam.TeamName}** in the **{challengerTeam.League}** League has been successfully canceled.")
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
                .WithDescription($"An admin, **{context.User.GlobalName ?? context.User.Username}**, has initiated a challenge in the **{challengerTeam.League}** League.")
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
                .WithDescription($"The challenge sent by **{challengerTeam.TeamName}** in the **{challengerTeam.League}** League has been successfully canceled by an admin.")
                .AddField("Admin", context.User.GlobalName ?? context.User.Username, inline: true)
                .AddField("Team", $"{challengerTeam.TeamName} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("League", challengerTeam.League, inline: true)
                .WithFooter("Challenge canceled successfully by Admin")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

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

        public Embed ReportWinSuccessEmbed(Team winningTeam, Team losingTeam, bool rankChange, string division)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏆 Match Result Reported!")
                .WithColor(Color.Green)
                .WithDescription(rankChange
                    ? $"Team **{winningTeam.TeamName}** has won the challenge they initiated against **{losingTeam.TeamName}** in the **{division} Division** and taken their rank of **#{winningTeam.Rank}**! Team **{losingTeam.TeamName}** drops down to **#{losingTeam.Rank}**. All other ranks have been adjusted accordingly."
                    : $"Team **{winningTeam.TeamName}** has defeated **{losingTeam.TeamName}** in the **{division} Division** and defended their rank. No rank changes occurred.")
                .AddField("Winning Team", $"{winningTeam.TeamName} (Rank #{winningTeam.Rank})", inline: true)
                .AddField("Losing Team", $"{losingTeam.TeamName} (Rank #{losingTeam.Rank})", inline: true)
                .WithFooter("Match successfully reported")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed ReportWinAdminSuccessEmbed(SocketInteractionContext context, Team winningTeam, Team losingTeam, bool rankChange, string division)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("🏆 Match Result Reported By Admin!")
                .WithColor(Color.Green)
                .WithDescription(rankChange
                    ? $"Team **{winningTeam.TeamName}** has won the challenge they initiated against **{losingTeam.TeamName}** in the **{division} Division** and taken their rank of **#{winningTeam.Rank}**! Team **{losingTeam.TeamName}** drops down to **#{losingTeam.Rank}**. All other ranks have been adjusted accordingly. This report was created by an Admin (**{context.User.GlobalName ?? context.User.Username}**) "
                    : $"Team **{winningTeam.TeamName}** has defeated **{losingTeam.TeamName}** in the **{division} Division** and defended their rank. No rank changes occurred.")
                .AddField("Winning Team", $"{winningTeam.TeamName} (Rank #{winningTeam.Rank})", inline: true)
                .AddField("Losing Team", $"{losingTeam.TeamName} (Rank #{losingTeam.Rank})", inline: true)
                .WithFooter("Match successfully reported by Admin")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

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
    }
}
