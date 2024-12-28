using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Ladderbot4.Data;
using Ladderbot4.Models;

namespace Ladderbot4.Managers
{
    public class LeagueManager
    {
        private readonly LeagueData _leagueData;

        private LeaguesByDivision _leaguesByDivision;

        public LeagueManager(LeagueData leagueData)
        {
            _leagueData = leagueData;
            _leaguesByDivision = _leagueData.LoadAllLeagues();
        }

        public void SaveLeagues()
        {
            _leagueData.SaveLeagues(_leaguesByDivision);
        }

        public void LoadLeaguesDatabase()
        {
            _leaguesByDivision = _leagueData.LoadAllLeagues();
        }

        public void SaveAndReloadLeaguesDatabase()
        {
            SaveLeagues();
            LoadLeaguesDatabase();
        }

        public bool IsLeagueNameUnique(string leagueName)
        {
            foreach (var division in new[] { _leaguesByDivision.Leagues1v1, _leaguesByDivision.Leagues2v2, _leaguesByDivision.Leagues3v3 })
            {
                foreach (League league in division)
                {
                    // Compare team names (case-insensitive)
                    if (league.LeagueName.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                    {
                        return false; // Name is not unique
                    }
                }
            }
            return true;
        }

        public bool IsTeamNameUnique(string teamName)
        {
            foreach (var division in new[] { _leaguesByDivision.Leagues1v1, _leaguesByDivision.Leagues2v2, _leaguesByDivision.Leagues3v3 })
            {
                foreach (League league in division)
                {
                    foreach (Team team in league.Teams)
                    {
                        // Compare team names (case-insensitive)
                        if (team.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                        {
                            return false; // Name is not unique
                        }
                    }
                }
            }

            return true;
        }

        public bool IsTeamsInSameLeague(League league, Team teamOne, Team teamTwo)
        {
            foreach (Team team in league.Teams)
            {
                if (teamOne.League.Equals(teamTwo.League, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
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

        public LeaguesByDivision GetAllLeagues()
        {
            return _leaguesByDivision;
        }

        public League GetLeagueByName(string leagueName)
        {
            foreach (var league in _leaguesByDivision.Leagues1v1)
            {
                if (league.LeagueName.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                    return league;
            }

            foreach (var league in _leaguesByDivision.Leagues2v2)
            {
                if (league.LeagueName.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                    return league;
            }

            foreach (var league in _leaguesByDivision.Leagues3v3)
            {
                if (league.LeagueName.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                    return league;
            }

            return null;
        }

        public League GetLeagueFromTeamName(string teamName)
        {
            League correctLeague;
            foreach (League league in _leaguesByDivision.Leagues1v1)
            {
                foreach (Team team in league.Teams)
                {
                    if (team.TeamName.Equals(teamName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        correctLeague = league;
                        return league;
                    }
                }
            }

            foreach (League league in _leaguesByDivision.Leagues2v2)
            {
                foreach (Team team in league.Teams)
                {
                    if (team.TeamName.Equals(teamName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        correctLeague = league;
                        return league;
                    }
                }
            }

            foreach (League league in _leaguesByDivision.Leagues3v3)
            {
                foreach (Team team in league.Teams)
                {
                    if (team.TeamName.Equals(teamName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        correctLeague = league;
                        return league;
                    }
                }
            }

            return null;
        }

        public Team? GetTeamByNameFromLeagues(string teamName)
        {
            // Search all leagues by division
            foreach (var leagueList in new[] { _leaguesByDivision.Leagues1v1, _leaguesByDivision.Leagues2v2, _leaguesByDivision.Leagues3v3 })
            {
                foreach (var league in leagueList)
                {
                    // Find the team by name
                    var team = league.Teams.FirstOrDefault(t => t.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase));
                    if (team != null)
                        return team; // Return the exact reference
                }
            }
            return null; // Return null if no match found
        }

        public void AddNewTeamToLeague(Team newTeam, League league)
        {
            _leagueData.AddTeamToLeague(newTeam, league);

            LoadLeaguesDatabase();
        }

        public void RemoveTeamFromLeague(Team team, League league)
        {
            _leagueData.RemoveTeamFromLeague(team, league);

            // Load newest save
            LoadLeaguesDatabase();
        }

        public League CreateLeagueObject(string leagueName, string leagueDivision)
        {
            return new League(leagueName, leagueDivision);
        }

        public void AddNewLeague(League league)
        {
            _leagueData.AddLeague(league);

            LoadLeaguesDatabase();
        }

        public void RemoveLeague(string leagueName, string division)
        {
            _leagueData.RemoveLeague(leagueName, division);

            LoadLeaguesDatabase();
        }
    }
}
