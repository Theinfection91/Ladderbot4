using Discord;
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
    }
}
