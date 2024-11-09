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
        private readonly string _filePath = Path.Combine("Data", "JsonFiles", "teams.json");

        public TeamData()
        {
            InitializeFile();
        }

        // Initialize the JSON file if it doesn't exist
        private void InitializeFile()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(new List<Team>(), Formatting.Indented));
            }
        }

        // Read teams from the JSON file
        public List<Team> LoadAllTeams()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Team>>(json);
        }

        // Write the given list of teams to the JSON file
        public void SaveTeams(List<Team> teams)
        {
            var json = JsonConvert.SerializeObject(teams, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        // Add a new team to the JSON file
        public void AddTeam(Team newTeam)
        {
            var teams = LoadAllTeams();
            teams.Add(newTeam);
            SaveTeams(teams);
        }
    }
}
