using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ladderbot4.Models;
using Newtonsoft.Json;

namespace Ladderbot4.Data
{
    public class LeagueData
    {
        private string _filePath;

        public LeagueData()
        {
            SetFilePath();
            InitializeFile();
        }

        private void SetFilePath()
        {
            // Set the file path relative to the base directory of the executable
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construct a path in a "Data/JsonFiles" folder within the base directory
            _filePath = Path.Combine(appBaseDirectory, "Databases", "leagues.json");

            // Ensure the directory exists
            string? directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);  // Create the directory if it doesn't exist
                Console.WriteLine($"Directory created: {directory}");
            }
        }

        private void InitializeFile()
        {
            if (!File.Exists(_filePath))
            {
                var initialData = new LeaguesByDivision
                {
                    Leagues1v1 = [],
                    Leagues2v2 = [],
                    Leagues3v3 = []
                };

                File.WriteAllText(_filePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        public LeaguesByDivision LoadAllLeagues()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<LeaguesByDivision>(json);
        }

        public void SaveLeagues(LeaguesByDivision leaguesByDivision)
        {
            var json = JsonConvert.SerializeObject(leaguesByDivision, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void AddLeague(League newLeague)
        {
            var leaguesByDivision = LoadAllLeagues();

            switch (newLeague.Format)
            {
                case "1v1":
                    leaguesByDivision.Leagues1v1.Add(newLeague);
                    break;

                case "2v2":
                    leaguesByDivision.Leagues2v2.Add(newLeague);
                    break;

                case "3v3":
                    leaguesByDivision.Leagues3v3.Add(newLeague);
                    break;
            }

            SaveLeagues(leaguesByDivision);
        }

        public void RemoveLeague(string leagueName, string division)
        {
            LeaguesByDivision leaguesByDivision = LoadAllLeagues();

            List<League>? divisionLeagues = division switch
            {
                "1v1" => leaguesByDivision.Leagues1v1,
                "2v2" => leaguesByDivision.Leagues2v2,
                "3v3" => leaguesByDivision.Leagues3v3,
                _ => null
            };

            if (divisionLeagues == null)
            {
                Console.WriteLine($"Invalid division: {division}");
                return;
            }

            // Find League
            League? leagueToRemove = divisionLeagues.FirstOrDefault(l => l.Name.Equals(leagueName, StringComparison.OrdinalIgnoreCase));

            if (leagueToRemove == null)
            {
                Console.WriteLine($"The League by the name of '{leagueName}' was not found.");
                return;
            }

            divisionLeagues.Remove(leagueToRemove);
            SaveLeagues(leaguesByDivision);
        }

        public void AddTeamToLeague(Team newTeam, League chosenLeague)
        {
            var leaguesByDivision = LoadAllLeagues();

            switch (newTeam.LeagueFormat)
            {
                case "1v1":
                    foreach (var league in leaguesByDivision.Leagues1v1)
                    {
                        if (league.Name.Equals(chosenLeague.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            league.AddTeam(newTeam);
                        }
                    }
                    break;

                case "2v2":
                    foreach (var league in leaguesByDivision.Leagues2v2)
                    {
                        if (league.Name.Equals(chosenLeague.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            league.AddTeam(newTeam);
                        }
                    }
                    break;

                case "3v3":
                    foreach (var league in leaguesByDivision.Leagues3v3)
                    {
                        if (league.Name.Equals(chosenLeague.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            league.AddTeam(newTeam);
                        }
                    }
                    break;
            }

            SaveLeagues(leaguesByDivision);
        }

        public void RemoveTeamFromLeague(Team team, League chosenLeague)
        {
            // Load all leagues from storage
            var leaguesByDivision = LoadAllLeagues();

            // Get the leagues list based on the team's division
            List<League> relevantLeagues = team.LeagueFormat switch
            {
                "1v1" => leaguesByDivision.Leagues1v1,
                "2v2" => leaguesByDivision.Leagues2v2,
                "3v3" => leaguesByDivision.Leagues3v3,
                _ => null
            };

            if (relevantLeagues == null)
            {
                Console.WriteLine($"Invalid division: {team.LeagueFormat}");
                return;
            }

            // Find the matching league
            var league = relevantLeagues.FirstOrDefault(l =>
                l.Name.Equals(chosenLeague.Name, StringComparison.OrdinalIgnoreCase));

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

            // Save the updated leagues
            SaveLeagues(leaguesByDivision);

            Console.WriteLine($"Team '{team.Name}' removed from league '{league.Name}', and ranks updated.");
        }

    }
}
