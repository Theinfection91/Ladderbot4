using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Challenge
    {
        public string Challenger {  get; set; }
        public int ChallengerRank { get; set; }
        public string Challenged {  get; set; }
        public int ChallengedRank { get; set; }
        public DateTime CreatedOn { get; set; }

        public Challenge(string challenger, int challengerRank, string challenged, int challengedRank)
        {
            Challenger = challenger;
            ChallengerRank = challengerRank;
            Challenged = challenged;
            ChallengedRank = challengedRank;
            CreatedOn = DateTime.Now;
        }

        public override bool Equals(object? obj)
        {
            // Check if the object is a Challenge
            if (obj is Challenge otherChallenge)
            {
                return this.Challenger == otherChallenge.Challenger && this.Challenged == otherChallenge.Challenged;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Combine hash codes of Format and Challenger to match override Equals logic
            return HashCode.Combine(Challenger, Challenged);
        }
    }
}
