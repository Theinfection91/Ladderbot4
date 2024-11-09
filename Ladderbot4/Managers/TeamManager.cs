using Ladderbot4.Data;
using Ladderbot4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Managers
{
    public class TeamManager
    {
        private readonly TeamData _teamData;

        private TeamsByDivision _teamsByDivision;

        public TeamManager(TeamData teamData)
        {
            _teamData = teamData;
            _teamsByDivision = _teamData.LoadAllTeams();
        }

        // Check if the team name is unique across all divisions
        public bool IsTeamNameUnique(string teamName)
        {
            // Load all teams grouped by division
            var teamsByDivision = _teamData.LoadAllTeams();

            // Check each division for the team name
            foreach (var division in new[] { teamsByDivision.Division1v1, teamsByDivision.Division2v2, teamsByDivision.Division3v3 })
            {
                // Iterate through the teams in this division
                foreach (Team team in division)
                {
                    // Compare team names (case-insensitive)
                    if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    {
                        return false; // Name is not unique
                    }
                }
            }

            // If no match was found, the name is unique
            return true;
        }

        public bool IsValidDivisionType(string divisionType)
        {
            List<string> validDivisionTypes = ["1v1", "2v2", "3v3"];

            foreach (string division in validDivisionTypes)
            {
                if (divisionType.ToLower().Equals(division))
                {
                    return true;
                }
            }
            return false;
        }

        public int GetTeamCount(string division)
        {
            switch (division)
            {
                case "1v1":
                    return _teamsByDivision.Division1v1.Count;

                case "2v2":
                    return _teamsByDivision.Division2v2.Count;

                case "3v3":
                    return _teamsByDivision.Division3v3.Count;

                default:
                    throw new ArgumentException($"Invalid division type given: {division}");
            }
            ;
        }

        public Team CreateTeamObject(string teamName, string division, int rank, List<Member> members, int wins = 0, int losses = 0)
        {
            return new Team(teamName, division, rank, wins, losses, members);
        }

        public void AddNewTeam(Team newTeam)
        {
            _teamData.AddTeam(newTeam);
        }
    }
}
