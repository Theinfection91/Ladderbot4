using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models.Achievements
{
    public class YourFirstLoss : Achievement
    {
        public YourFirstLoss() : base()
        {
            Id = 2;
            Name = "Your First Loss";
            Description = "Lose your first match in any division";
            AchievementPointsValue = 5;
        }
    }
}
