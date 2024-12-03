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

        public Embed RegisterTeamSuccessEmbed(Team newTeam)
        {
            var embedBuilder = new EmbedBuilder()
            .WithTitle("🎉 Team Registered Successfully!")
            .WithColor(Color.Green)
            .WithDescription($"A new team has been created in the **{newTeam.Division} Division**!")
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

        public Embed RemoveTeamSuccessEmbed(Team team)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("✅ Team Removed Successfully")
                .WithColor(Color.Green)
                .WithDescription($"The team **{team.TeamName}** has been successfully removed from the **{team.Division} Division**.")
                .AddField("Division", team.Division, inline: true)
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
                .WithDescription($"A new challenge has been initiated in the **{challengerTeam.Division} Division**!")
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
                .WithDescription($"The challenge sent by **{challengerTeam.TeamName}** in the **{challengerTeam.Division} Division** has been successfully canceled.")
                .AddField("Team", $"{challengerTeam.TeamName} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("Division", challengerTeam.Division, inline: true)
                .WithFooter("Challenge canceled successfully.")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

        public Embed AdminChallengeSuccessEmbed(Team challengerTeam, Team challengedTeam, SocketInteractionContext context)
        {
            var embedBuilder = new EmbedBuilder()
                .WithTitle("⚔️ Admin-Initiated Challenge!")
                .WithColor(Color.Green)
                .WithDescription($"An admin, **{context.User.GlobalName ?? context.User.Username}**, has initiated a challenge in the **{challengerTeam.Division} Division**.")
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
                .WithDescription($"The challenge sent by **{challengerTeam.TeamName}** in the **{challengerTeam.Division} Division** has been successfully canceled by an admin.")
                .AddField("Admin", context.User.GlobalName ?? context.User.Username, inline: true)
                .AddField("Team", $"{challengerTeam.TeamName} (Rank #{challengerTeam.Rank})", inline: true)
                .AddField("Division", challengerTeam.Division, inline: true)
                .WithFooter("Challenge canceled successfully by Admin")
                .WithTimestamp(DateTimeOffset.Now);

            return embedBuilder.Build();
        }

    }
}
