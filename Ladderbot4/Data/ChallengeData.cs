using Ladderbot4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ladderbot4.Data
{
    public class ChallengeData
    {
        private readonly string _filePath;

        public ChallengeData()
        {
            _filePath = SetFilePath();
            InitializeFile();
        }

        // Set the file path for challenges.json
        private string SetFilePath()
        {
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(appBaseDirectory, "Databases", "challenges.json");

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Console.WriteLine($"Directory created: {directory}");
            }

            return filePath;
        }

        // Initialize the JSON file if it doesn't exist
        private void InitializeFile()
        {
            if (!File.Exists(_filePath))
            {
                var initialData = new ChallengesByDivision();
                SaveChallenges(initialData);
            }
        }

        // Load all challenges from the JSON file
        public ChallengesByDivision LoadAllChallenges()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<ChallengesByDivision>(json) ?? new ChallengesByDivision();
        }

        // Save challenges to the JSON file
        public void SaveChallenges(ChallengesByDivision challengesByDivision)
        {
            var json = JsonConvert.SerializeObject(challengesByDivision, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        // Add a new challenge
        public void AddChallenge(string division, string leagueName, Challenge challenge)
        {
            var challenges = LoadAllChallenges();
            challenges.AddChallenge(division, leagueName, challenge);
            SaveChallenges(challenges);
        }

        // Remove a challenge
        public void RemoveChallenge(string division, string leagueName, Predicate<Challenge> match)
        {
            var challenges = LoadAllChallenges();
            challenges.RemoveChallenge(division, leagueName, match);
            SaveChallenges(challenges);
        }

        // Sudo remove challenges where the team is involved
        public void SudoRemoveChallenge(string division, string leagueName, string teamName)
        {
            RemoveChallenge(division, leagueName, challenge =>
                challenge.Challenger.Equals(teamName, StringComparison.OrdinalIgnoreCase) ||
                challenge.Challenged.Equals(teamName, StringComparison.OrdinalIgnoreCase));
        }

        // Get all challenges for a specific league
        public List<Challenge> GetChallenges(string division, string leagueName)
        {
            var challenges = LoadAllChallenges();
            return challenges.GetChallenges(division, leagueName);
        }
    }
}
