using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltimateCC;
using System.IO;

public static class PlayerSaver
{
    private static string path = Application.dataPath + Path.AltDirectorySeparatorChar + "PlayerData.json";
    private static string persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "PlayerData.json";

    public static void SavePlayerData()
    {
        string savePath = path;
        Debug.Log("saving data at: " + savePath);
        string json = JsonUtility.ToJson(PlayerMain.Instance.PlayerData);

        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
    }

    public static void LoadPlayerData()
    {
        string loadPath = path;
        using StreamReader reader =new StreamReader(loadPath);
        string json = reader.ReadToEnd();
        
        PlayerMain.Instance.PlayerData = JsonUtility.FromJson<PlayerData>(json);
    }

}
