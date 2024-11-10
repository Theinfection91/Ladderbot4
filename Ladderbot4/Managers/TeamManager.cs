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

        public void LoadTeamsDatabase()
        {
            _teamsByDivision = _teamData.LoadAllTeams();
        }
        
        // Check if two given teams are in the same division
        public bool IsTeamsInSameDivision(Team teamOne, Team teamTwo)
        {

            return teamOne.Division == teamTwo.Division;
        }

        // Check if the team name is unique across all divisions
        public bool IsTeamNameUnique(string teamName)
        {
            // Check each division for the team name
            foreach (var division in new[] { _teamsByDivision.Division1v1, _teamsByDivision.Division2v2, _teamsByDivision.Division3v3 })
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
            foreach (string division in new[] { "1v1", "2v2", "3v3" })
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
            return division switch
            {
                "1v1" => _teamsByDivision.Division1v1.Count,
                "2v2" => _teamsByDivision.Division2v2.Count,
                "3v3" => _teamsByDivision.Division3v3.Count,
                _ => throw new ArgumentException($"Invalid division type given: {division}"),
            };
            ;
        }

        public List<Team> GetTeamsByDivision(string division)
        {
            return division switch
            {
                "1v1" => _teamsByDivision.Division1v1,
                "2v2" => _teamsByDivision.Division2v2,
                "3v3" => _teamsByDivision.Division3v3,
                _ => throw new ArgumentException($"Invalid division type given: {division}"),
            };
            ;
        }

        public Team GetTeamByName(string teamName)
        {
            // Search in Division1v1
            foreach (var team in _teamsByDivision.Division1v1)
            {
                if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    return team;
            }

            // Search in Division2v2
            foreach (var team in _teamsByDivision.Division2v2)
            {
                if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    return team;
            }

            // Search in Division3v3
            foreach (var team in _teamsByDivision.Division3v3)
            {
                if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    return team;
            }

            // If no team is found
            return null;
        }

        public Team CreateTeamObject(string teamName, string division, int rank, List<Member> members, int wins = 0, int losses = 0)
        {
            return new Team(teamName, division, rank, wins, losses, members);
        }

        public void AddNewTeam(Team newTeam)
        {
            _teamData.AddTeam(newTeam);

            // Loads newest save of the database to backing field
            LoadTeamsDatabase();
        }

        public void RemoveTeam(string teamName, string division)
        {
            _teamData.RemoveTeam(teamName, division);

            // Loads newest save of the database to backing field
            LoadTeamsDatabase();
        }
    }
}
