using Ladderbot4.Data;
using Ladderbot4.Models;
using Ladderbot4.Models.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class AchievementManager
    {
        // Will work off of MemberData
        private readonly MemberData _memberData;

        private MembersList _membersList;

        public AchievementManager(MemberData memberData)
        {
            _memberData = memberData;
            _membersList = _memberData.LoadAllMembers();
        }

        public void SaveMembersAchievements()
        {
            _memberData.SaveAllMembers(_membersList);
        }

        public void LoadMembersAchievements()
        {
            _membersList = _memberData.LoadAllMembers();
        }

        public void SaveAndReloadMembersAchievements()
        {
            SaveMembersAchievements();
            LoadMembersAchievements();
        }

        public bool IsAchievementUnlockedOnMember(Member member, Achievement achievement)
        {
            foreach (Achievement unlockedAchievement in member.UnlockedAchievements)
            {
                if (unlockedAchievement == achievement)
                    return true;
            }
            return false;
        }

        public void AddAchievementToMember(Member member, Achievement achievement)
        {
            // Add specific achievement to specific member
            member.UnlockedAchievements.Add(achievement);

            // Have the member recalculate their total achievement points
            member.UpdateTotalAchievementPoints();

            // Save and Reload the members.json
            SaveAndReloadMembersAchievements();
        }
    }
}
