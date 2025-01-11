using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Models
{
    public class ChallengesHub
    {
        public Dictionary<string, List<Challenge>> Challenges { get; set; }

        public ChallengesHub()
        {
            Challenges = new Dictionary<string, List<Challenge>>();
        }

        // Ensure a league exists
        public void EnsureLeagueExists(string leagueName)
        {
            if (!Challenges.ContainsKey(leagueName))
                Challenges[leagueName] = new List<Challenge>();
        }

        // Get all challenges for a specific league
        public List<Challenge> GetChallenges(string leagueName)
        {
            if (Challenges.TryGetValue(leagueName, out var leagueChallenges))
            {
                return leagueChallenges;
            }

            return new List<Challenge>();
        }

        // Add a challenge to a specific league
        public void AddChallenge(string leagueName, Challenge challenge)
        {
            EnsureLeagueExists(leagueName);
            Challenges[leagueName].Add(challenge);
        }

        // Remove a challenge by criteria
        public void RemoveChallenge(string leagueName, Predicate<Challenge> match)
        {
            if (Challenges.TryGetValue(leagueName, out var leagueChallenges))
            {
                leagueChallenges.RemoveAll(match);
            }
        }
    }
}
