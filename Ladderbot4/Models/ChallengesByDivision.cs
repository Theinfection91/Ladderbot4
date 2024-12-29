using System;
using System.Collections.Generic;

namespace Ladderbot4.Models
{
    public class ChallengesByDivision
    {
        public Dictionary<string, Dictionary<string, List<Challenge>>> Challenges { get; set; }

        public ChallengesByDivision()
        {
            Challenges = new Dictionary<string, Dictionary<string, List<Challenge>>>
            {
                { "1v1", new Dictionary<string, List<Challenge>>() },
                { "2v2", new Dictionary<string, List<Challenge>>() },
                { "3v3", new Dictionary<string, List<Challenge>>() }
            };
        }

        // Helper Methods

        // Ensure a league exists for a given division
        public void EnsureLeagueExists(string division, string leagueName)
        {
            if (!Challenges.ContainsKey(division))
                Challenges[division] = new Dictionary<string, List<Challenge>>();

            if (!Challenges[division].ContainsKey(leagueName))
                Challenges[division][leagueName] = new List<Challenge>();
        }

        // Get all challenges for a specific league
        public List<Challenge> GetChallenges(string division, string leagueName)
        {
            if (Challenges.TryGetValue(division, out var leagues) &&
                leagues.TryGetValue(leagueName, out var leagueChallenges))
            {
                return leagueChallenges;
            }

            return new List<Challenge>();
        }

        // Add a challenge to a specific league
        public void AddChallenge(string division, string leagueName, Challenge challenge)
        {
            EnsureLeagueExists(division, leagueName);
            Challenges[division][leagueName].Add(challenge);
        }

        // Remove a challenge by criteria
        public void RemoveChallenge(string division, string leagueName, Predicate<Challenge> match)
        {
            if (Challenges.TryGetValue(division, out var leagues) &&
                leagues.TryGetValue(leagueName, out var leagueChallenges))
            {
                leagueChallenges.RemoveAll(match);
            }
        }
    }
}
