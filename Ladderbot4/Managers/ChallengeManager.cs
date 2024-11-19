using Ladderbot4.Data;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class ChallengeManager
    {
        private readonly ChallengeData _challengeData;

        private ChallengesByDivision _challengesByDivision;

        public ChallengeManager(ChallengeData challengeData)
        {
            _challengeData = challengeData;
            _challengesByDivision = _challengeData.LoadAllChallenges();
        }

        public void LoadChallengesDatabase()
        {
            _challengesByDivision = _challengeData.LoadAllChallenges();
        }

        public void SaveChallengesDatabase()
        {
            _challengeData.SaveChallenges(_challengesByDivision);
        }

        public void SaveAndReloadChallenges()
        {
            SaveChallengesDatabase();
            LoadChallengesDatabase();
        }

        public Challenge? GetChallengeByTeamObject(Team team)
        {
            // Load the challenges database
            LoadChallengesDatabase();

            string teamName = team.TeamName;
            List<Challenge> challenges = team.Division switch
            {
                "1v1" => _challengesByDivision.Challenges1v1,
                "2v2" => _challengesByDivision.Challenges2v2,
                "3v3" => _challengesByDivision.Challenges3v3,
                _ => null
            };

            // Iterate over the challenges in the specified division
            foreach (var challenge in challenges)
            {
                // Check if the team is either the Challenger or Challenged
                if ((challenge.Challenger != null && challenge.Challenger.Equals(teamName, StringComparison.OrdinalIgnoreCase)) ||
                    (challenge.Challenged != null && challenge.Challenged.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
                {
                    return challenge;
                }
            }

            // Return null if no challenge is found
            return null;
        }

        public bool IsTeamAwaitingChallengeMatch(Team team)
        {
            LoadChallengesDatabase();

            switch (team.Division)
            {
                case "1v1":
                    foreach (Challenge challenge in _challengesByDivision.Challenges1v1)
                    {
                        if (challenge.Challenger == team.TeamName || challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "2v2":
                    foreach (Challenge challenge in _challengesByDivision.Challenges2v2)
                    {
                        if (challenge.Challenger == team.TeamName || challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "3v3":
                    foreach (Challenge challenge in _challengesByDivision.Challenges3v3)
                    {
                        if (challenge.Challenger == team.TeamName || challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

            }
            return false;
        }

        public bool IsTeamChallenger(Team team)
        {
            LoadChallengesDatabase();

            switch (team.Division)
            {
                case "1v1":
                    foreach (Challenge challenge in _challengesByDivision.Challenges1v1)
                    {
                        if (challenge.Challenger == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "2v2":
                    foreach (Challenge challenge in _challengesByDivision.Challenges2v2)
                    {
                        if (challenge.Challenger == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "3v3":
                    foreach (Challenge challenge in _challengesByDivision.Challenges3v3)
                    {
                        if (challenge.Challenger == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public bool IsTeamChallenged(Team team)
        {
            LoadChallengesDatabase();

            switch (team.Division)
            {
                case "1v1":
                    foreach (Challenge challenge in _challengesByDivision.Challenges1v1)
                    {
                        if (challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "2v2":
                    foreach (Challenge challenge in _challengesByDivision.Challenges2v2)
                    {
                        if (challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;

                case "3v3":
                    foreach (Challenge challenge in _challengesByDivision.Challenges3v3)
                    {
                        if (challenge.Challenged == team.TeamName)
                        {
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }

        public bool IsTeamChallengeable(Team challengerTeam, Team challengedTeam)
        {
            LoadChallengesDatabase();

            return challengerTeam.Rank > challengedTeam.Rank && challengerTeam.Rank <= challengedTeam.Rank + 2;
        }

        public string GetChallengesData(string division)
        {
            // Load database
            LoadChallengesDatabase();

            List<Challenge> challenges = GetChallengesByDivision(division);
            StringBuilder sb = new();

            sb.AppendLine($"```\n");
            foreach (Challenge challenge in challenges)
            {
                sb.AppendLine($"Challenger Team: {challenge.Challenger} - Challenged Team: {challenge.Challenged} - Created: {challenge.CreatedOn}\n");
            }
            sb.AppendLine("\n```");

            return sb.ToString();
        }

        public List<Challenge> GetChallengesByDivision(string division)
        {
            return division switch
            {
                "1v1" => _challengesByDivision.Challenges1v1,
                "2v2" => _challengesByDivision.Challenges2v2,
                "3v3" => _challengesByDivision.Challenges3v3,
                _ => throw new ArgumentException($"Invalid division type given: {division}"),
            };
            ;
        }

        public Challenge CreateChallengeObject(string division, string challenger, string challenged)
        {
            LoadChallengesDatabase();

            return new Challenge(division, challenger, challenged);
        }

        public void AddNewChallenge(Challenge challenge)
        {
            _challengeData.AddChallenge(challenge);

            // Load the newly saved Challenges database
            LoadChallengesDatabase();
        }

        public void RemoveChallenge(string challengerTeam, string division)
        {
            _challengeData.RemoveChallenge(challengerTeam, division);

            // Load the newly saved challenges database
            LoadChallengesDatabase();
        }

        public void SudoRemoveChallenge(string teamName, string division)
        {
            _challengeData.SudoRemoveChallenge(teamName, division);

            // Load the newly saved challenges database
            LoadChallengesDatabase();
        }
    }
}
