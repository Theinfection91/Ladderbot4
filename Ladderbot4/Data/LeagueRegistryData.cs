using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Models;

namespace Ladderbot4.Data
{
    public class LeagueRegistryData : Data<LeagueRegistry>
    {
        public LeagueRegistryData() : base("leagues.json", "Databases")
        {

        }

        public void AddLeague(League newLeague)
        {
            LeagueRegistry leagueRegistry = Load();

            if (leagueRegistry != null)
            {
                leagueRegistry.LeagueList.Add(newLeague);

                Save(leagueRegistry);
            }
        }
    }
}
