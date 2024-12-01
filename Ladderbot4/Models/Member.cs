using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Member
    {
        public ulong DiscordId { get; set; }
        public string DisplayName { get; set; }

        // TODO - Stats and Achievements
            // 1v1
        public int Wins1v1 { get; set; } = 0;
        public int Losses1v1 { get; set; } = 0;
        public int TeamCount1v1 { get; set; } = 0;

            // 2v2
        public int Wins2v2 { get; set; } = 0;
        public int Losses2v2 { get; set; } = 0;
        public int TeamCount2v2 { get; set; } = 0;

            // 3v3
        public int Wins3v3 { get; set; } = 0;
        public int Losses3v3 { get; set; } = 0;
        public int TeamCount3v3 { get; set; } = 0;

        // Derived Stats
        public int MatchCount { get; set; } = 0;
        public int TotalTeamCount { get; set; } = 0;
        public double WinRatio1v1 => (Wins1v1 + Losses1v1) == 0 ? 0 : (double)Wins1v1 / (Wins1v1 + Losses1v1);
        public double WinRatio2v2 => (Wins2v2 + Losses2v2) == 0 ? 0 : (double)Wins2v2 / (Wins2v2 + Losses2v2);
        public double WinRatio3v3 => (Wins3v3 + Losses3v3) == 0 ? 0 : (double)Wins3v3 / (Wins3v3 + Losses3v3);

        public Member(ulong discordId, string displayName)
        {
            DiscordId = discordId;
            DisplayName = displayName;
        }

        public override bool Equals(object? obj)
        {
            // Check if the object is a Member
            if (obj is Member otherMember)
            {
                return this.DiscordId == otherMember.DiscordId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return DiscordId.GetHashCode(); // Use DiscordId for hash code
        }

        public void UpdateMatchCount()
        {
            MatchCount = Wins1v1 + Losses1v1 + Wins2v2 + Losses2v2 + Wins3v3 + Losses3v3;
        }

        public void UpdateTotalTeamCount()
        {
            TotalTeamCount = TeamCount1v1 + TeamCount2v2 + TeamCount3v3;
        }
    }
}
