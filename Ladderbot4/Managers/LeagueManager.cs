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
        private readonly LeagueRegistryData _leagueRegistryData;

        private LeaguesByDivision _leaguesByDivision;
        private LeagueRegistry _leagueRegistry;

        public LeagueManager(LeagueData leagueData, LeagueRegistryData leagueRegistryData)
        {
            // Phase out OLD LeagueData
            _leagueData = leagueData;
            // Introduce and migrate to NEW LeagueRegistryData
            _leagueRegistryData = leagueRegistryData;
            
            // Load OLD leagues.json - Phase out
            _leaguesByDivision = _leagueData.LoadAllLeagues();
            // Load NEW league_registry.json
            _leagueRegistry = _leagueRegistryData.Load();
        }

        public void SaveLeagues()
        {
            _leagueData.SaveLeagues(_leaguesByDivision);
        }

        public void SaveLeagueRegistry()
        {
            _leagueRegistryData.Save(_leagueRegistry);
        }

        public void LoadLeaguesDatabase()
        {
            _leaguesByDivision = _leagueData.LoadAllLeagues();
        }

        public void LoadLeagueRegistry()
        {
            _leagueRegistry = _leagueRegistryData.Load();
        }

        public void SaveAndReloadLeaguesDatabase()
        {
            SaveLeagues();
            LoadLeaguesDatabase();
        }

        public void SaveAndReloadLeagueRegistry()
        {
            SaveLeagueRegistry();
            LoadLeagueRegistry();
        }

        public IEnumerable<League> GetAllLeagues()
        {
            // Combine leagues from all divisions into a single list
            return _leaguesByDivision.Leagues1v1
                .Concat(_leaguesByDivision.Leagues2v2)
                .Concat(_leaguesByDivision.Leagues3v3);
        }

        public bool IsLeagueNameUnique(string leagueName)
        {
            foreach (var division in new[] { _leaguesByDivision.Leagues1v1, _leaguesByDivision.Leagues2v2, _leaguesByDivision.Leagues3v3 })
            {
                foreach (League league in division)
                {
                    // Compare team names (case-insensitive)
                    if (league.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                    {
                        return false; // Name is not unique
                    }
                }
            }
            return true;
        }

        public bool IsXvXLeagueNameUnique(string leagueName)
        {
            foreach (League league in _leagueRegistry.Leagues)
            {
                if (league.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                {
                    return false; // Name is not unique
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
                        if (team.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                        {
                            return false; // Name is not unique
                        }
                    }
                }
            }

            return true;
        }

        public bool IsXvXTeamNameUnique(string teamName)
        {
            foreach (League league in _leagueRegistry.Leagues)
            {
                foreach (Team team in league.Teams)
                {
                    if (team.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
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

        public List<League> GetLeaguesByDivisionType(string divisionType)
        {
            // Grab every league in the program's memory
            IEnumerable<League> allLeagues = GetAllLeagues();

            // Init list to add to
            List<League> divisionLeagues = [];

            // Iterate and find each League with specified division type and add to list.
            foreach (League league in allLeagues)
            {
                if (league.Format.Equals(divisionType, StringComparison.OrdinalIgnoreCase))
                {
                    divisionLeagues.Add(league);
                }
            }
            // Return the list
            return divisionLeagues;
        }

        public List<League> GetAllLeaguesAsList()
        {
            // Grab every league in the program's memory
            IEnumerable<League> allLeagues = GetAllLeagues();

            List<League> leaguesAsList = [];

            foreach (League league in allLeagues)
            {
                leaguesAsList.Add(league);
            }
            return leaguesAsList;
        }

        public League GetXvXLeagueByName(string leagueName)
        {
            foreach (League league in _leagueRegistry.Leagues)
            {
                if (league.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                {
                    return league;
                }
            }
            return null;
        }

        public League GetLeagueByName(string leagueName)
        {
            foreach (var league in _leaguesByDivision.Leagues1v1)
            {
                if (league.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                    return league;
            }

            foreach (var league in _leaguesByDivision.Leagues2v2)
            {
                if (league.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                    return league;
            }

            foreach (var league in _leaguesByDivision.Leagues3v3)
            {
                if (league.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
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
                    if (team.Name.Equals(teamName.Trim(), StringComparison.OrdinalIgnoreCase))
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
                    if (team.Name.Equals(teamName.Trim(), StringComparison.OrdinalIgnoreCase))
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
                    if (team.Name.Equals(teamName.Trim(), StringComparison.OrdinalIgnoreCase))
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
                    var team = league.Teams.FirstOrDefault(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase));
                    if (team != null)
                        return team; // Return the exact reference
                }
            }
            return null; // Return null if no match found
        }

        public string ConvertTeamSizeToDivisionTagStr(int teamSize)
        {
            return $"{teamSize.ToString()}v{teamSize.ToString()}";
        }

        public void AddNewTeamToLeague(Team newTeam, League league)
        {
            _leagueData.AddTeamToLeague(newTeam, league);

            LoadLeaguesDatabase();
        }

        public void AddXvXTeamToLeague(Team team, League league)
        {
            _leagueRegistryData.AddTeamToLeague(team, league);

            LoadLeaguesDatabase();
        }

        public void RemoveTeamFromLeague(Team team, League league)
        {
            _leagueData.RemoveTeamFromLeague(team, league);

            // Load newest save
            LoadLeaguesDatabase();
        }

        public League CreateXvXLeagueObject(string leagueName, string leagueDivision, int teamSize)
        {
            return new League(leagueName, leagueDivision)
            {
                TeamSize = teamSize
            };
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

        public void AddNewXvXLeague(League league)
        {
            _leagueRegistryData.AddLeague(league);

            LoadLeagueRegistry();
        }

        public void RemoveLeague(string leagueName, string division)
        {
            _leagueData.RemoveLeague(leagueName, division);

            LoadLeaguesDatabase();
        }

        public void DeleteXvXLeague(string leagueName)
        {
            _leagueRegistryData.RemoveLeague(leagueName);

            LoadLeagueRegistry();
        }
    }
}
