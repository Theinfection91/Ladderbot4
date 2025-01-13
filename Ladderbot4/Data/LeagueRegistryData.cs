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
        public LeagueRegistryData() : base("league_registry.json", "Databases")
        {

        }

        public void AddLeague(League newLeague)
        {
            LeagueRegistry leagueRegistry = Load();

            if (leagueRegistry != null)
            {
                leagueRegistry.Leagues.Add(newLeague);

                Save(leagueRegistry);
            }
        }

        public void RemoveLeague(string leagueName)
        {
            LeagueRegistry leagueRegistry = Load();

            // Find League
            League? leagueToRemove = leagueRegistry.Leagues.FirstOrDefault(l => l.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase));

            if (leagueToRemove == null)
            {
                Console.WriteLine($"The League by the name of '{leagueName}' was not found.");
                return;
            }

            // Remove given League and save
            leagueRegistry.Leagues.Remove(leagueToRemove);
            Save(leagueRegistry);
        }

        public void AddTeamToLeague(Team team, League chosenLeague)
        {
            LeagueRegistry leagueRegistry = Load();

            foreach (League league in leagueRegistry.Leagues)
            {
                if (league.Name.Equals(chosenLeague.Name, StringComparison.OrdinalIgnoreCase))
                {
                    league.AddTeam(team);
                }
            }
            Save(leagueRegistry);
        }

        public void RemoveTeamFromLeague(Team team, League chosenLeague)
        {
            LeagueRegistry leagueRegistry = Load();

            // Find League
            League? league = leagueRegistry.Leagues.FirstOrDefault(l => l.Name.Equals(chosenLeague.Name, StringComparison.OrdinalIgnoreCase));

            // Remove team from League
            league.RemoveTeam(team);

            // Reassign ranks only if teams remain
            if (league.Teams.Any())
            {
                league.Teams.Sort((a, b) => a.Rank.CompareTo(b.Rank));
                for (int i = 0; i < league.Teams.Count; i++)
                {
                    league.Teams[i].Rank = i + 1;
                }
            }

            // Save the League Registry
            Save(leagueRegistry);

            Console.WriteLine($"DEBUG - LeagueRegistryData - Team '{team.Name}' removed from League '{league.Name}', and ranks updated.");
        }
    }
}
