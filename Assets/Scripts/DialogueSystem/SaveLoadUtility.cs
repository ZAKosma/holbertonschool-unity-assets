using System.IO;
using UnityEngine;

namespace DialogueSystem
{
    public static class SaveLoadUtility
    {
        public static void SaveGame()
        {
            GameSaveData saveData = new GameSaveData();

            // ... Fill saveData with current progress ...
            string json = JsonUtility.ToJson(saveData);

            string path = Application.persistentDataPath + "/save.json";
            int i = 0;
            while (File.Exists(path))
            {
                i++;
                path = Application.persistentDataPath + "/save" + i + ".json";
            }

            File.WriteAllText(path, json);
        }
    
        public static void LoadGame()
        {
            if (File.Exists(Application.persistentDataPath + "/save.json"))
            {
                string json = File.ReadAllText(Application.persistentDataPath + "/save.json");
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                // ... Set current progress from saveData ...
            }
        }

    }
}