using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataSaver : MonoBehaviour
{
    public static DataSaver Instance { get; private set; }

    private string saveFilePath = "savegame.json"; // Path to save the file


    private void Awake()
    {
        Instance = this;
    }

    // Save data to a file
    public void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);  // Convert SaveData to JSON string
        File.WriteAllText(saveFilePath, json);  // Write JSON to file
        Debug.Log("Game saved!");
    }

    // Load data from a file
    public SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);  // Read the file content
            SaveData data = JsonUtility.FromJson<SaveData>(json);  // Convert JSON to SaveData object
            Debug.Log("Game loaded!");
            return data;
        }
        else
        {
            Debug.LogWarning("No save file found!");
            return null;
        }
    }
}
