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

        public bool IsTeamAwaitingChallengeMatch(Team team)
        {
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
            return challengerTeam.Rank > challengedTeam.Rank && challengerTeam.Rank <= challengedTeam.Rank + 2;
        }

        public Challenge CreateChallengeObject(string division, string challenger, string challenged)
        {
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
