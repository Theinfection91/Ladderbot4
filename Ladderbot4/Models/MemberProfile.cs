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
        public int ExperienceToNextLevel => (GetNextLevelAmount(Level) - Experience);

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
            return DiscordId.GetHashCode();
        }

        public void AddExperience(int amount)
        {
            Experience += amount;
            CheckLevelUp();
        }

        public int GetNextLevelAmount(int currentLevel)
        {
            return (int)(50 * Math.Pow(currentLevel, 1.2));
        }

        public void CheckLevelUp()
        {
            int nextLevelAmount = GetNextLevelAmount(Level);
            if (Experience >= nextLevelAmount)
            {
                Level++;
                Title = GetTitle(Level);

                // TODO: Inform user of level up and title

            }
        }

        public string GetTitle(int level)
        {
            if (level >= 1 && level < 3)
                return MemberTitlesEnum.Novice.ToString();
            else if (level >= 3 && level < 5)
                return MemberTitlesEnum.Apprentice.ToString();
            else if (level >= 5 && level < 7)
                return MemberTitlesEnum.Challenger.ToString();
            else if (level >= 7 && level < 9)
                return MemberTitlesEnum.Contender.ToString();
            else if (level >= 9 && level < 11)
                return MemberTitlesEnum.Elite.ToString();
            else if (level >= 11 && level < 13)
                return MemberTitlesEnum.Champion.ToString();
            else if (level >= 13 && level < 15)
                return MemberTitlesEnum.Master.ToString();
            else if (level >= 15 && level < 20)
                return MemberTitlesEnum.Master.ToString();
            else if (level >= 20)
                return MemberTitlesEnum.Legend.ToString();
            else
                return "Invalid level given";
        }
    }
}
