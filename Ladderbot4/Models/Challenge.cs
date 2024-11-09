using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Challenge
    {
        public string Id { get; set; }
        public string Division { get; set; }
        public string Challenger {  get; set; }
        public string Challenged {  get; set; }

        public Challenge(string division, string challenger, string challenged)
        {
            Id = challenger;
            Division = division;
            Challenger = challenger;
            Challenged = challenged;
        }
    }
}
