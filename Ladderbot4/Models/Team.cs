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
        public string Division { get; set; }
        public int Rank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public List<Member> Members { get; set; }

        // TODO - Stats and Achievements
        // Streaks
        public int WinStreak { get; set; }
        public int LoseStreak { get; set; }

        // W/L Ratio
        public double WinRatio => (Wins + Losses) == 0 ? 0 : (double)Wins / (Wins + Losses);

        public Team(string teamName, string division, int rank, int wins, int losses, List<Member> members)
        {
            TeamName = teamName;
            Division = division;
            Rank = rank;
            Wins = wins;
            Losses = losses;
            Members = members;
        }

        public string GetAllMemberNamesToStr()
        {
            switch (Members.Count)
            {
                case 1:
                    return $"{Members[0].DisplayName}";

                case 2:
                    return $"{Members[0].DisplayName}, {Members[1].DisplayName}";

                case 3:
                    return $"{Members[0].DisplayName}, {Members[1].DisplayName}, {Members[1].DisplayName}";

                default:
                    return $"Incorrect member count.";
            }
        }
    }
}
