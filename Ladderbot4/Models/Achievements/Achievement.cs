using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models.Achievements
{
    public abstract class Achievement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AchievementPointsValue { get; set; }
        public DateTime AchievedOn { get; set; }

        public Achievement()
        {
            AchievedOn = DateTime.Now;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Achievement otherAchievement)
            {
                return this.Id == otherAchievement.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode(); // Use Id for hash code
        }
    }    
}
