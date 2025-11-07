using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Serializable snapshot of the minimal game state we persist to disk.
/// </summary>
[Serializable]
public struct GameSave
{
    public int leftScore;
    public int rightScore;
    public float masterVolume;       // 0..1
    public float aiResponsiveness;   // 0.4..1.2 (as configured)
    public string savedAtIso;
}

/// <summary>
/// Lightweight JSON-based save/load helper.
/// Writes a single file to Application.persistentDataPath.
/// </summary>
public static class SaveSystem
{
    public static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    /// <summary>
    /// Writes the provided <see cref="GameSave"/> to disk as JSON.
    /// </summary>
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

    /// <summary>
    /// Attempts to load a previously-saved snapshot.
    /// </summary>
    /// <param name="data">Out parameter populated on success.</param>
    /// <returns>True if a save file existed and was read successfully.</returns>
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
