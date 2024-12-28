using System;
using System.Collections.Generic;

namespace Ladderbot4.Models
{
    public class LeaguesByDivision
    {
        public List<League> Leagues1v1 { get; set; }
        public List<League> Leagues2v2 { get; set; }
        public List<League> Leagues3v3 { get; set; }

        public LeaguesByDivision()
        {
            Leagues1v1 = new List<League>();
            Leagues2v2 = new List<League>();
            Leagues3v3 = new List<League>();
        }

        // Combine all leagues into a single collection for iteration
        public IEnumerable<League> GetAllLeagues()
        {
            foreach (var league in Leagues1v1) yield return league;
            foreach (var league in Leagues2v2) yield return league;
            foreach (var league in Leagues3v3) yield return league;
        }
    }
}
