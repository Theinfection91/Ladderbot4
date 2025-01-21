using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Models.Achievements;

namespace Ladderbot4.Models
{
    public class MemberProfile
    {
        public ulong DiscordId { get; set; }
        public string DisplayName { get; set; }

        // Basic Stats
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int LeagueChampionships { get; set; } = 0;

        // Derived Stats
        public int TotalMatchCount { get; set; } = 0;
        public int TotalTeamCount { get; set; } = 0;
        public double WinLossRatio => (Wins + Losses) == 0 ? 0 : (double)Wins / (Wins + Losses);

        public MemberProfile(ulong discordId, string displayName)
        {
            DiscordId = discordId;
            DisplayName = displayName;
        }

        public override bool Equals(object? obj)
        {
            // Check if the object is a Member
            if (obj is MemberProfile otherMember)
            {
                return this.DiscordId == otherMember.DiscordId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return DiscordId.GetHashCode(); // Use DiscordId for hash code
        }
    }
}
