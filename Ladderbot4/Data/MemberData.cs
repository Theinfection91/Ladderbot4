using Ladderbot4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladderbot4.Data
{
    public class MemberData
    {
        private string _filePath;

        public MemberData()
        {
            SetFilePath();
            InitializeFile();
        }

        private void SetFilePath()
        {
            // Set the file path relative to the base directory of the executable
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Construct a path in a "Data/JsonFiles" folder within the base directory
            _filePath = Path.Combine(appBaseDirectory, "Databases", "members.json");

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
                var initialData = new MembersList
                {
                    AllMembers = []
                };

                File.WriteAllText(_filePath, JsonConvert.SerializeObject(initialData, Formatting.Indented));
            }
        }

        public MembersList LoadAllMembers()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<MembersList>(json);
        }

        public void SaveAllMembers(MembersList membersList)
        {
            var json = JsonConvert.SerializeObject(membersList, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }

        public void AddMember(Member member)
        {
            var membersList = LoadAllMembers();

            membersList.AllMembers.Add(member);

            SaveAllMembers(membersList);
        }
    }
}
