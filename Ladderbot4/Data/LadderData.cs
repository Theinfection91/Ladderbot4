using Ladderbot4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Data
{
    public class LadderData
    {
        private string _filePath;

        public LadderData()
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
            _filePath = Path.Combine(appBaseDirectory, "Databases", "states.json");

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
                var initialData = new StatesByDivision();

                File.WriteAllText(_filePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        public StatesByDivision LoadAllStates()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<StatesByDivision>(json);
        }

        public void SaveAllStates(StatesByDivision statesByDivision)
        {
            var json = JsonConvert.SerializeObject(statesByDivision, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void AddState(State newState)
        {
            var statesByDivision = LoadAllStates();

            switch (newState.Division)
            {
                case "1v1":
                    statesByDivision.States1v1.Add(newState);
                    break;

                case "2v2":
                    statesByDivision.States2v2.Add(newState);
                    break;

                case "3v3":
                    statesByDivision.States3v3.Add(newState);
                    break;
            }

            SaveAllStates(statesByDivision);
        }

        public void RemoveState(string leagueName, string division)
        {
            StatesByDivision statesByDivision = LoadAllStates();

            List<State>? divisionStates = division switch
            {
                "1v1" => statesByDivision.States1v1,
                "2v2" => statesByDivision.States2v2,
                "3v3" => statesByDivision.States3v3,
                _ => null
            };

            if (divisionStates == null || divisionStates.Count == 0)
            {
                Console.WriteLine($"{DateTime.Now} StatesManager - Null error when trying to remove a State for the given League name: {leagueName}");
            }

            // Find the correct state
            State? stateToRemove = divisionStates.FirstOrDefault(s => s.LeagueName.Equals(leagueName, StringComparison.OrdinalIgnoreCase));

            if (stateToRemove == null)
            {
                Console.WriteLine($"{DateTime.Now} StatesManager - The State with the League name of '{leagueName}' was not found.");
                return;
            }

            divisionStates.Remove(stateToRemove);
            SaveAllStates(statesByDivision);
        }
    }
}
