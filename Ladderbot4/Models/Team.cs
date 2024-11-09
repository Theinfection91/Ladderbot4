using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Division { get; set; }
        public int Rank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public List<Member> Members { get; set; }

        public Team(int id, string teamName, string division, int rank, int wins, int losses, List<Member> members)
        {
            Id = id;
            TeamName = teamName;
            Division = division;
            Rank = rank;
            Wins = wins;
            Losses = losses;
            Members = members;
        }
    }
}
