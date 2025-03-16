using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Models;

namespace Ladderbot4.Data
{
    public class ChallengesHubData : Data<ChallengesHub>
    {
        public ChallengesHubData() : base("challenges_hub.json", "Databases")
        {

        }

        public void AddChallenge(string leagueName, Challenge challenge)
        {
            ChallengesHub challengesHub = Load();
            challengesHub.AddChallenge(leagueName, challenge);
            Save(challengesHub);
        }

        public void RemoveChallenge(string leagueName, Predicate<Challenge> challenge)
        {
            ChallengesHub challengesHub = Load();
            challengesHub.RemoveChallenge(leagueName, challenge);
            Save(challengesHub);
        }

        public void SudoRemoveChallenge(string leagueName, string teamName)
        {
            RemoveChallenge(leagueName, challenge =>
            challenge.Challenger.Equals(teamName, StringComparison.OrdinalIgnoreCase) ||
            challenge.Challenged.Equals(teamName, StringComparison.OrdinalIgnoreCase));
        }

        public void RemoveLeagueFromChallenges(string leagueName)
        {
            ChallengesHub challengesHub = Load();

            // Check if the league name exists in Challenges
            if (challengesHub.Challenges.ContainsKey(leagueName))
            {
                challengesHub.Challenges.Remove(leagueName);
                Save(challengesHub);
            }
        }
    }
}
