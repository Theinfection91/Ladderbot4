using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Challenge
    {
        public string Division { get; set; }
        public string Challenger {  get; set; }
        public string Challenged {  get; set; }
        public DateTime CreatedOn { get; set; }

        public Challenge(string division, string challenger, string challenged)
        {
            Division = division;
            Challenger = challenger;
            Challenged = challenged;
            CreatedOn = DateTime.Now;
        }

        public override bool Equals(object? obj)
        {
            // Check if the object is a Member
            if (obj is Challenge otherChallenge)
            {
                return this.Division == otherChallenge.Division && this.Challenger == otherChallenge.Challenger;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Combine hash codes of Division and Challenger to match override Equals logic
            return HashCode.Combine(Division, Challenger);
        }
    }
}
