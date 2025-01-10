using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class ChallengeHub
    {
        public List<Challenge> Challenges { get; set; }

        public ChallengeHub()
        {
            Challenges = [];
        }
    }
}
