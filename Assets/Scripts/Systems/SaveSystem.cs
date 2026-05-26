using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "savedata.json");

    public static SaveData Load()
    {
        if (!File.Exists(FilePath))
            return new SaveData();

        try
        {
            string json = File.ReadAllText(FilePath);
            return JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }
        catch
        {
            return new SaveData();
        }
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }
}
