using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class ChallengesByDivision
    {
        public List<Challenge> Challenges1v1 { get; set; }
        public List<Challenge> Challenges2v2 { get; set; }
        public List<Challenge> Challenges3v3 { get; set; }

        public ChallengesByDivision()
        {
            Challenges1v1 = [];
            Challenges2v2 = [];
            Challenges3v3 = [];
        }
    }
}
