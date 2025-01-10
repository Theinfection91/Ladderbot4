using Newtonsoft.Json;
using System;
using System.IO;

namespace Ladderbot4.Data
{
    public abstract class Data<T> where T : new()
    {
        private readonly string _filePath;

        protected Data(string fileName, string folderName)
        {
            _filePath = SetFilePath(fileName, folderName);
            InitializeFile();
        }

        private string SetFilePath(string fileName, string folderName)
        {
            Console.WriteLine("Set File Path in base Data");
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = Path.Combine(appBaseDirectory, folderName, fileName);

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Console.WriteLine($"Directory created: {directory}");
            }

            return filePath;
        }

        private void InitializeFile()
        {
            if (!File.Exists(_filePath))
            {
                Save(new T());
            }
        }

        public T Load()
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<T>(json) ?? new T();
        }

        public void Save(T data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}
