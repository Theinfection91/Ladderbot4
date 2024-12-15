using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
