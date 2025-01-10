using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class LeagueRegistry
    {
        public List<League> LeagueList { get; set; }

        public LeagueRegistry()
        {
            LeagueList = new List<League>();
        }
    }
}
