using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Team
    {
        // Basic Info
        public string Name { get; set; }
        public string League { get; set; }
        public int Size { get; set; }
        public string LeagueFormat { get; set; }
        public int Rank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public List<Member> Members { get; set; }

        // Streaks
        public int WinStreak { get; set; }
        public int LoseStreak { get; set; }

        // Display if a team has been "Challenged" or is "Free"
        public bool IsChallengeable { get; set; }

        // W/L Ratio
        public double WinRatio => (Wins + Losses) == 0 ? 0 : (double)Wins / (Wins + Losses);

        public Team(string teamName, string league, string leagueFormat, int rank, int wins, int losses, List<Member> members)
        {
            Name = teamName;
            League = league;
            LeagueFormat = leagueFormat;
            Rank = rank;
            Wins = wins;
            Losses = losses;
            Members = members;
            IsChallengeable = true;
        }

        public string GetAllMemberNamesToStr()
        {
            StringBuilder sb = new();
            foreach (Member m in Members)
            {
                sb.Append($"{m.DisplayName}, ");
            }

            // Remove the last comma and space
            if (sb.Length > 2)
            {
                sb.Length -= 2;
            }

            return sb.ToString();
        }

        public bool IsTeamFull()
        {
            return Size == Members.Count;
        }
    }
}
