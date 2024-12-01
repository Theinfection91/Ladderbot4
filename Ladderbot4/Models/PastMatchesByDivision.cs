using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class PastMatchesByDivision
    {
        public List<Match> History1v1 { get; set; }
        public List<Match> History2v2 { get; set; }
        public List<Match> History3v3 { get; set; }

        public PastMatchesByDivision()
        {
            History1v1 = [];
            History2v2 = [];
            History3v3 = [];
        }
    }
}
