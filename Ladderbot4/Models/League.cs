using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class League
    {
        public string Id { get; set; }
        public string LeagueName { get; set; }
        public string Division { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<Team> Teams { get; set; }

        public League(string leagueName, string leagueDivision)
        {
            LeagueName = leagueName;
            Division = leagueDivision;
            CreatedOn = DateTime.Now;
            Teams = [];
        }

        public void AddTeam(Team team)
        {
            if (team != null)
            {
                Teams.Add(team);
            }
        }
    }
}
