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
        private readonly LeagueRegistryData _leagueRegistryData;

        private LeagueRegistry _leagueRegistry;

        public LeagueManager(LeagueRegistryData leagueRegistryData)
        {
            _leagueRegistryData = leagueRegistryData;
            _leagueRegistry = _leagueRegistryData.Load();
        }

        public void SaveLeagueRegistry()
        {
            _leagueRegistryData.Save(_leagueRegistry);
        }

        public void LoadLeagueRegistry()
        {
            _leagueRegistry = _leagueRegistryData.Load();
        }

        public void SaveAndReloadLeagueRegistry()
        {
            SaveLeagueRegistry();
            LoadLeagueRegistry();
        }

        public bool IsLeagueNameUnique(string leagueName)
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

        public bool IsLeagueNameUnique(string leagueName, bool caseSensitive)
        {
            foreach (League league in _leagueRegistry.Leagues)
            {
                if (caseSensitive)
                {
                    if (league.Name.Equals(leagueName))
                    {
                        return false; // Name is not unique
                    }
                }
                else
                {
                    // Case-insensitive comparison
                    if (league.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase))
                    {
                        return false; // Name is not unique
                    }
                }
            }
            return true;
        }

        public bool IsTeamNameUnique(string teamName)
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

        public bool IsTeamNameUnique(string teamName, bool caseSensitivity)
        {
            foreach (League league in _leagueRegistry.Leagues)
            {
                foreach (Team team in league.Teams)
                {
                    if (caseSensitivity)
                    {
                        if (team.Name.Equals(teamName))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (team.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
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

        public List<League> GetAllLeagues()
        {
            return _leagueRegistry.Leagues;
        }

        public League GetLeagueByName(string leagueName)
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

        public League GetLeagueFromTeamName(string teamName)
        {
            foreach (League league in _leagueRegistry.Leagues)
            {
                foreach (Team team in league.Teams)
                {
                    if (team.Name.Equals(teamName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return league;
                    }
                }
            }
            return null;
        }

        public Team? GetTeamByNameFromLeagues(string teamName)
        {
            foreach (League league in _leagueRegistry.Leagues)
            {
                Team? team = league.Teams.FirstOrDefault(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase));
                if (team != null)
                    return team;
            }
            return null;
        }

        public string ConvertTeamSizeToDivisionTag(int teamSize)
        {
            return $"{teamSize.ToString()}v{teamSize.ToString()}";
        }

        public void AddTeamToLeague(Team team, League league)
        {
            _leagueRegistryData.AddTeamToLeague(team, league);

            LoadLeagueRegistry();
        }

        public void RemoveTeamFromLeague(Team team, League league)
        {
            _leagueRegistryData.RemoveTeamFromLeague(team, league);

            LoadLeagueRegistry();
        }

        public League CreateLeagueObject(string leagueName, string leagueDivision, int teamSize)
        {
            return new League(leagueName, leagueDivision)
            {
                TeamSize = teamSize
            };
        }

        public void AddNewLeague(League league)
        {
            _leagueRegistryData.AddLeague(league);

            LoadLeagueRegistry();
        }

        public void DeleteLeague(string leagueName)
        {
            _leagueRegistryData.RemoveLeague(leagueName);

            LoadLeagueRegistry();
        }
    }
}
