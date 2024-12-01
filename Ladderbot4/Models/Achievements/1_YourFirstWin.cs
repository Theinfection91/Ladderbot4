using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models.Achievements
{
    public class YourFirstWin : Achievement
    {
        public YourFirstWin() : base()
        {
            Id = 1;
            Name = "Your First Win";
            Description = "Win your first match in any division.";
            AchievementPointsValue = 5;
        }
    }
}
