using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class Match
    {
        public int MatchId { get; set; }
        public string Division { get; set; }
        public string Challenger { get; set; }
        public string Challenged { get; set; }
        public string Winner { get; set; }
        public string Loser { get; set; }
        public int ChallengerRank { get; set; }
        public int ChallengedRank { get; set; }
        public DateTime ChallengeDate { get; set; }
        public DateTime CompleteDate { get; set; }

        public Match(int matchId, string division, string challenger, string challenged, string winner, string loser, int challengerRank, int challengedRank, DateTime challengeDate)
        {
            MatchId = matchId;
            Division = division;
            Challenger = challenger;
            Challenged = challenged;
            Winner = winner;
            Loser = loser;
            ChallengerRank = challengerRank;
            ChallengedRank = challengedRank;
            ChallengeDate = challengeDate;
            CompleteDate = DateTime.Now;
        }
    }
}
