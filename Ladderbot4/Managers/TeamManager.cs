using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class TeamManager
    {
        public List<Team> AllRegisteredTeams { get; set; }
        public List<Team> OneVersusOneTeams { get; set; }
        public List<Team> TwoVersusTwoTeams { get; set; }
        public List<Team> ThreeVersusThreeTeams { get; set; }

        public TeamManager()
        {

        }

        public bool IsTeamNameUnique(string teamName)
        {
            return false;
        }

        public Team CreateTeamObject(int id, string teamName, string division, int rank, int wins, int losses, List<Member> members)
        {
            return new Team(teamName, division, rank, wins, losses, members);
        }
    }
}
