using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class State
    {
        public string LeagueName { get; set; }
        public bool IsLadderRunning { get; set; } = false;
        public ulong ChallengesChannelId { get; set; } = 0;
        public ulong StandingsChannelId { get; set; } = 0;
        public ulong TeamsChannelId { get; set; } = 0;

        public State()
        {

        }
    }
}
