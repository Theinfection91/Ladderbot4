using Ladderbot4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Data
{
    public class HistoryData
    {
        private string _filePath;

        public HistoryData()
        {
            SetFilePath();

            InitializeFile();
        }

        private void SetFilePath()
        {
            // Set the file path relative to the base directory of the executable
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construct a path in a "Data/JsonFiles" folder within the base directory
            _filePath = Path.Combine(appBaseDirectory, "Databases", "history.json");

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
                var initialData = new PastMatchesByDivision
                {
                    History1v1 = [],
                    History2v2 = [],
                    History3v3 = [],
                };

                File.WriteAllText(_filePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        public PastMatchesByDivision LoadAllPastMatches()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<PastMatchesByDivision>(json);
        }

        public void SaveAllPastMatches(PastMatchesByDivision pastMatchesByDivision)
        {
            var json = JsonConvert.SerializeObject(pastMatchesByDivision, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void AddMatch(Match match)
        {
            var pastMatchesByDivision = LoadAllPastMatches();

            // Add match to correct division
            switch (match.Division)
            {
                case "1v1":
                    pastMatchesByDivision.History1v1.Add(match);
                    break;

                case "2v2":
                    pastMatchesByDivision.History2v2.Add(match);
                    break;

                case "3v3":
                    pastMatchesByDivision.History3v3.Add(match);
                    break;
            }

            SaveAllPastMatches(pastMatchesByDivision);
        }
    }
}
