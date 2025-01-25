using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Enums;
using Ladderbot4.Models.Achievements;

namespace Ladderbot4.Models
{
    public class MemberProfile
    {
        public ulong DiscordId { get; set; }
        public string DisplayName { get; set; }

        // TODO: Title, Level and XP System
        public string Title {  get; set; } = MemberTitlesEnum.Novice.ToString();
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;

        // Basic Stats
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int LeagueChampionships { get; set; } = 0;

        // Derived Stats
        public int TotalMatchCount { get; set; } = 0;
        public int TotalSeasons { get; set; } = 0;
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

        public void AddExperience(int amount)
        {
            Experience += amount;
            CheckLevelUp();
        }

        public int GetNextLevelAmount(int currentLevel)
        {
            return Level * 50;
        }

        public void CheckLevelUp()
        {
            int nextLevelAmount = GetNextLevelAmount(Level);
            if (Experience >= nextLevelAmount)
            {
                Level++;
                // TODO: Title check
            }
        }        
    }
}
