using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class StatesAtlas
    {
        public List<State> States { get; set; }
        public ulong LeaguesChannelId { get; set; }
        public ulong LeaguesMessageId { get; set; }

        public StatesAtlas()
        {
            States = [];
            LeaguesChannelId = 0;
            LeaguesMessageId = 0;
        }
    }
}
