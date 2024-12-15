using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class LeaguesByDivision
    {
        public List<League> Leagues1v1 { get; set; }
        public List<League> Leagues2v2 { get; set; }
        public List<League> Leagues3v3 { get; set; }

        public LeaguesByDivision()
        {
            Leagues1v1 = [];
            Leagues2v2 = [];
            Leagues3v3 = [];
        }
    }
}
