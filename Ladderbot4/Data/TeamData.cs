using Ladderbot4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Data
{
    public class TeamData
    {
        private string _filePath;

        public TeamData()
        {
            SetFilePath();
            InitializeFile();
        }

        // Correctly sets the file path for matching json
        private void SetFilePath()
        {
            // Set the file path relative to the base directory of the executable
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construct a path in a "Data/JsonFiles" folder within the base directory
            _filePath = Path.Combine(appBaseDirectory, "Databases", "teams.json");

            // Ensure the directory exists
            string? directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);  // Create the directory if it doesn't exist
                Console.WriteLine($"Directory created: {directory}");
            }
        }

        // Initialize the JSON file if it doesn't exist
        private void InitializeFile()
        {
            if (!File.Exists(_filePath))
            {
                var initialData = new TeamsByDivision
                {
                    Division1v1 = [],
                    Division2v2 = [],
                    Division3v3 = []
                };

                File.WriteAllText(_filePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        // Read teams from the JSON file
        public TeamsByDivision LoadAllTeams()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<TeamsByDivision>(json);
        }

        // Write the given list of teams to the JSON file
        public void SaveTeams(TeamsByDivision teamsByDivision)
        {
            Console.WriteLine("Saving to file: " + _filePath);
            var json = JsonConvert.SerializeObject(teamsByDivision, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        // Add a new team to the JSON file
        public void AddTeam(Team newTeam)
        {
            var teamsByDivision = LoadAllTeams();

            // Add the team to the appropriate division
            switch (newTeam.Division)
            {
                case "1v1":
                    teamsByDivision.Division1v1.Add(newTeam);
                    break;
                case "2v2":
                    teamsByDivision.Division2v2.Add(newTeam);
                    break;
                case "3v3":
                    teamsByDivision.Division3v3.Add(newTeam);
                    break;
            }

            // Save the updated list of teams back to the file
            SaveTeams(teamsByDivision);
        }

        public void RemoveTeam(string teamName, string division)
        {
            // Load teams database
            TeamsByDivision teamsByDivision = LoadAllTeams();

            List<Team>? divisionTeams = division switch
            {
                "1v1" => teamsByDivision.Division1v1,
                "2v2" => teamsByDivision.Division2v2,
                "3v3" => teamsByDivision.Division3v3,
                _ => null
            };

            if (divisionTeams == null)
            {
                Console.WriteLine($"Invalid division: {division}");
                return;
            }

            // Find the team to remove
            Team? teamToRemove = divisionTeams.FirstOrDefault(t => t.TeamName.Equals(teamName, StringComparison.OrdinalIgnoreCase));

            // Check if the team was found
            if (teamToRemove == null)
            {
                Console.WriteLine($"Team {teamName} not found in {division} division.");
                return;
            }

            // Grab team's rank
            int removedRank = teamToRemove.Rank;

            // Remove the team
            divisionTeams.Remove(teamToRemove);
            Console.WriteLine($"Team {teamName} removed from {division} division.");

            // Adjust ranks for the other teams
            foreach (Team team in divisionTeams)
            {
                if (team.Rank > removedRank)
                {
                    team.Rank--;
                }
            }

            // Save the updated teams database
            SaveTeams(teamsByDivision);
        }

    }
}
