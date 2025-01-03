﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Team
    {
        // Basic Info
        public string TeamName { get; set; }
        public string League { get; set; }
        public string Division { get; set; }
        public int Rank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public List<Member> Members { get; set; }

        // Stats and Achievements
        // Streaks
        public int WinStreak { get; set; }
        public int LoseStreak { get; set; }

        // Display if a team has been "Challenged" or is "Free"
        public bool IsChallengeable { get; set; }

        // W/L Ratio
        public double WinRatio => (Wins + Losses) == 0 ? 0 : (double)Wins / (Wins + Losses);

        public Team(string teamName, string league, string division, int rank, int wins, int losses, List<Member> members)
        {
            TeamName = teamName;
            League = league;
            Division = division;
            Rank = rank;
            Wins = wins;
            Losses = losses;
            Members = members;
            IsChallengeable = true;
        }

        public string GetAllMemberNamesToStr()
        {
            return Members.Count switch
            {
                1 => $"{Members[0].DisplayName}",
                2 => $"{Members[0].DisplayName}, {Members[1].DisplayName}",
                3 => $"{Members[0].DisplayName}, {Members[1].DisplayName}, {Members[2].DisplayName}",
                _ => $"Incorrect member count. Count: {Members.Count}",
            };
        }
    }
}
