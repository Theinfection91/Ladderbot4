﻿using Ladderbot4.Models;
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
            // Navigate to the root folder (assuming your .sln file is at the root of the project)
            string projectRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."); // Navigate up four levels
            projectRootPath = Path.GetFullPath(projectRootPath); // Make it an absolute path

            // Construct the full path to the teams.json file
            _filePath = Path.Combine(projectRootPath, "Ladderbot4", "Data", "JsonFiles", "teams.json");

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(_filePath);
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
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(new List<Team>(), Formatting.Indented));
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
            var json = JsonConvert.SerializeObject(teamsByDivision, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        // Add a new team to the JSON file
        public void AddTeam(Team newTeam)
        {
            var teamsByDivision = LoadAllTeams();

            // Add the team to the appropriate division
            if (newTeam.Division == "1v1")
            {
                teamsByDivision.Division1v1.Add(newTeam);
            }
            else if (newTeam.Division == "2v2")
            {
                teamsByDivision.Division2v2.Add(newTeam);
            }
            else if (newTeam.Division == "3v3")
            {
                teamsByDivision.Division3v3.Add(newTeam);
            }

            // Save the updated list of teams back to the file
            SaveTeams(teamsByDivision);
        }
    }
}
