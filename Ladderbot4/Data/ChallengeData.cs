using Ladderbot4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Data
{
    public class ChallengeData
    {
        private string _filePath;

        public ChallengeData()
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
            _filePath = Path.Combine(appBaseDirectory, "Data", "JsonFiles", "challenges.json");

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
                var initialData = new ChallengesByDivision
                {
                    Challenges1v1 = [],
                    Challenges2v2 = [],
                    Challenges3v3 = []
                };

                File.WriteAllText(_filePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        // Read challenges from the JSON file
        public ChallengesByDivision LoadAllChallenges()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<ChallengesByDivision>(json);
        }

        // Write the given list of challenges to the JSON file
        public void SaveChallenges(ChallengesByDivision challengesByDivision)
        {
            Console.WriteLine("Saving to file: " + _filePath);
            var json = JsonConvert.SerializeObject(challengesByDivision, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        // Add a new challenge to the JSON file
        public void AddChallenge(Challenge challenge)
        {
            var challengesByDivision = LoadAllChallenges();

            // Add the team to the appropriate division
            switch (challenge.Division)
            {
                case "1v1":
                    challengesByDivision.Challenges1v1.Add(challenge);
                    break;
                case "2v2":
                    challengesByDivision.Challenges2v2.Add(challenge);
                    break;
                case "3v3":
                    challengesByDivision.Challenges3v3.Add(challenge);
                    break;
            }

            // Save the updated list of teams back to the file
            SaveChallenges(challengesByDivision);
        }
    }
}
