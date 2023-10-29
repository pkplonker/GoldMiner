using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Save
{
    public class SaveFileHandler : ISaveLoadIO
    {
        private readonly string dataPath;
        private readonly string dataFileName;

        public SaveFileHandler(string path, string filename)
        {
            dataPath = path;
            dataFileName = filename;
        }

        public Dictionary<string, object> Load()
        {
            string fullPath = Path.Combine(dataPath, dataFileName);
            if (string.IsNullOrWhiteSpace(fullPath)) throw new Exception("No Filepath supplied");
            Dictionary<string, object> loadedData = new Dictionary<string, object>();
            if (File.Exists(fullPath))
            {
                try
                {
                    string json = File.ReadAllText(fullPath);
                    loadedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                }
                catch (Exception exception)
                {
                    Debug.LogError("Unable to load data from " + fullPath + "\n" + exception);
                    throw;
                }
            }

            Debug.LogWarning("Load successfully read");
            return loadedData;
        }

        public void Save(Dictionary<string, object> data)
        {
            string fullPath = Path.Combine(dataPath, dataFileName);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(fullPath, json);
            }
            catch (Exception exception)
            {
                Debug.LogError("Unable to save data to file " + fullPath + "\n" + exception);
                throw;
            }

            Debug.LogWarning($"Saved successfully {fullPath}");
        }

        public void Clear()
        {
            string fullPath = Path.Combine(dataPath, dataFileName);
            try
            {
                if (!File.Exists(fullPath)) return;
                File.Delete(fullPath);
            }
            catch (Exception exception)
            {
                Debug.LogError("Unable to clear save data file " + fullPath + "\n" + exception);
                throw;
            }
        }
    }
}
