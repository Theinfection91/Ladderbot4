using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class ChallengesHub
    {
        public List<Challenge> Challenges { get; set; }

        public ChallengesHub()
        {
            Challenges = [];
        }
    }
}
