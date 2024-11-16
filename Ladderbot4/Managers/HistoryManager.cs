using Ladderbot4.Data;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class HistoryManager
    {
        private HistoryData _historyData;

        public HistoryManager(HistoryData historyData)
        {
            _historyData = historyData;
        }

        public Match CreateMatchObject(int matchId, string division, string challenger, string challenged, string winner, string loser, int challengerRank, int challengedRank, DateTime challengeDate)
        {
            return new Match(matchId, division, challenger, challenged, winner, loser, challengerRank, challengedRank, challengeDate);
        }


    }
}
