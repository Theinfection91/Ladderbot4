using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Enums;

namespace Ladderbot4.Models
{
    public class LevelGuide
    {
        public Dictionary<ExperienceValuesEnum, int> ExperienceValues { get; private set; }

        public LevelGuide()
        {
            ExperienceValues = new Dictionary<ExperienceValuesEnum, int>
            {
                { ExperienceValuesEnum.WinMatch, 20 },
                { ExperienceValuesEnum.LoseMatch, 10 },
                { ExperienceValuesEnum.ParticipateSeason, 10 },
                { ExperienceValuesEnum.CompleteSeason, 20 },
                { ExperienceValuesEnum.FirstPlaceLadder, 75 },
                { ExperienceValuesEnum.SecondPlaceLadder, 50 },
                { ExperienceValuesEnum.ThirdPlaceLadder, 25 }
            };
        }

        public int GetExperienceForAction(ExperienceValuesEnum action)
        {
            if (ExperienceValues.ContainsKey(action))
            {
                return ExperienceValues[action];
            }
            else
            {
                return 0;
            }
        }
    }
}
