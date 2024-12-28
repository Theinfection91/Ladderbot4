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
        public PastMatchesByDivision PastMatchesByDivision;

        public HistoryManager(HistoryData historyData)
        {
            _historyData = historyData;
            PastMatchesByDivision = _historyData.LoadAllPastMatches();
        }

        public void SavePastMatches()
        {
            _historyData.SaveAllPastMatches(PastMatchesByDivision);
        }

        public void LoadPastMatches()
        {
            PastMatchesByDivision = _historyData.LoadAllPastMatches();
        }

        public void SaveAndReloadPastMatches()
        {
            SavePastMatches();
            LoadPastMatches();
        }

        public int GetDivisionMatchCount(string division)
        {
            switch (division)
            {
                case "1v1":
                    return PastMatchesByDivision.History1v1.Count;

                case "2v2":
                    return PastMatchesByDivision.History2v2.Count;

                case "3v3":
                    return PastMatchesByDivision.History3v3.Count;

                default:
                    return -1;
            }
        }

        public Match CreateMatchObject(int matchId, string division, string challenger, int challengerRank, string challenged, int challengedRank, string winner, string loser, DateTime challengeDate)
        {
            return new Match(matchId, division, challenger, challengerRank, challenged, challengedRank, winner, loser, challengeDate);
        }

        public void AddNewMatch(Match match)
        {
            _historyData.AddMatch(match);

            // Load the newest save
            LoadPastMatches();
        }
    }
}
