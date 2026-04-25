using System.IO;
using UnityEngine;

public static class ProfileManager
{
    private static string Folder => Application.persistentDataPath + "/profiles/";

    public static void SaveProfile(PlayerProfile profile)
    {
        if (!Directory.Exists(Folder))
            Directory.CreateDirectory(Folder);

        string json = JsonUtility.ToJson(profile, true);
        File.WriteAllText(Folder + profile.playerName + ".json", json);
    }

    public static PlayerProfile LoadProfile(string name)
    {
        string path = Folder + name + ".json";
        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<PlayerProfile>(json);
    }
    public static void DeleteProfile(string name)
    {
        string path = Folder + name + ".json";
        if (File.Exists(path))
            File.Delete(path);
    }

    public static string[] GetAllProfiles()
    {
        if (!Directory.Exists(Folder))
            return new string[0];

        var files = Directory.GetFiles(Folder, "*.json");
        for (int i = 0; i < files.Length; i++)
            files[i] = Path.GetFileNameWithoutExtension(files[i]);

        return files;
    }
}
