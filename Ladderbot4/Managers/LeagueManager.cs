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

        public League CreateLeagueObject(string leagueName, string leagueDivision)
        {
            return new League(leagueName, leagueDivision);
        }

        public void AddNewLeague(League league)
        {
            _leagueData.AddLeague(league);

            LoadLeaguesDatabase();
        }

        public void RemoveLeague(string leagueId, string division)
        {
            _leagueData.RemoveLeague(leagueId, division);

            LoadLeaguesDatabase();
        }
    }
}
