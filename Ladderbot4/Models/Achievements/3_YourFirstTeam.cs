using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models.Achievements
{
    public class YourFirstTeam : Achievement
    {
        public YourFirstTeam() : base()
        {
            Id = 3;
            Name = "Your First Team";
            Description = "Participate in your first team in any division";
            AchievementPointsValue = 5;
        }
    }
}
