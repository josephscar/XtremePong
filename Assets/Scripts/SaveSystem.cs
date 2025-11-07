using System;
using System.IO;
using UnityEngine;

[Serializable]
public struct GameSave
{
    public int leftScore;
    public int rightScore;
    public float masterVolume;       // 0..1
    public float aiResponsiveness;   // 0.4..1.2 (as configured)
    public string savedAtIso;
}

public static class SaveSystem
{
    public static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(GameSave data)
    {
        try
        {
            data.savedAtIso = DateTime.UtcNow.ToString("o");
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log($"Game saved to: {SavePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save game: {ex.Message}");
        }
    }

    public static bool TryLoad(out GameSave data)
    {
        try
        {
            if (!File.Exists(SavePath))
            {
                data = default;
                return false;
            }
            var json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<GameSave>(json);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load game: {ex.Message}");
            data = default;
            return false;
        }
    }
}

