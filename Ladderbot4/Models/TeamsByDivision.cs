using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class TeamsByDivision
    {
        public List<Team> Division1v1 { get; set; }
        public List<Team> Division2v2 { get; set; }
        public List<Team> Division3v3 { get; set; }

        public TeamsByDivision()
        {
            Division1v1 = [];
            Division2v2 = [];
            Division3v3 = [];
        }
    }
}
