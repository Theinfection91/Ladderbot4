using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class StatesByDivision
    {
        // 1v1 Specific
        public bool Is1v1LadderRunning { get; set; }
        public ulong ChallengesChannel1v1 {  get; set; }
        public ulong StandingsChannel1v1 { get; set; }
        public ulong TeamsChannel1v1 { get; set; }

        // 2v2 Specific
        public bool Is2v2LadderRunning { get; set; }
        public ulong ChallengesChannel2v2 { get; set; }
        public ulong StandingsChannel2v2 { get; set; }
        public ulong TeamsChannel2v2 { get; set; }

        // 3v3 Specific
        public bool Is3v3LadderRunning { get; set; }
        public ulong ChallengesChannel3v3 { get; set; }
        public ulong StandingsChannel3v3 { get; set; }
        public ulong TeamsChannel3v3 { get; set; }
    }
}
