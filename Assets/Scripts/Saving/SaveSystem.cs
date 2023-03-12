using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/tile.challenge";
    private static string filePath = Application.persistentDataPath;
    static string str;
    public static void UnlockLevel(int levelNumber)
    {
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream openStream = new FileStream(path, FileMode.Open);

            PlayerLevelData data = formatter.Deserialize(openStream) as PlayerLevelData;
            List<bool> levelsStatus = data.levelsStatus;
            if (levelNumber > levelsStatus.Count + 1)
            {
                Debug.LogWarning("Saving not successful");
                return;
            }
            else if (levelNumber == levelsStatus.Count + 1)
            {
                levelsStatus.Add(true);
            }
            else
            {
                levelsStatus[levelNumber - 1] = true;
            }
            data.levelsStatus = levelsStatus;
            openStream.Close();

            FileStream createStream = new FileStream(path, FileMode.Create);
            formatter.Serialize(createStream, data);
            createStream.Close();
        }
        else
        {
            Debug.Log("Source file not found!");
        }
    }

    public static PlayerLevelData LoadLevel(int levelCount)
    {
        if (File.Exists(path))
        {
            Debug.Log("Loading Level Data");

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerLevelData data = formatter.Deserialize(stream) as PlayerLevelData;

            stream.Close();

            while (data.levelsStatus.Count < levelCount)
            {
                data.levelsStatus.Add(false);
            }
            FileStream openStream = new FileStream(path, FileMode.Create);
            formatter.Serialize(openStream, data);

            openStream.Close();

            return data;
        }
        else
        {
            Debug.Log("Creating new Level Data");

            PlayerLevelData data = new PlayerLevelData();
            List<bool> result = new List<bool>(new bool[levelCount]);
            result[0] = true;
            data.levelsStatus = result;
            SaveLevelData(data);

            return data;
        }
    }

    static void SaveLevelData(PlayerLevelData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static List<int> GetLevelProgress()
    {
        List<int> result = new List<int>();
        List<bool> levelsStatus;
        int count = 0;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream;
        if(File.Exists(path))
        {
            stream = new FileStream(path, FileMode.Open);
            PlayerLevelData data = formatter.Deserialize(stream) as PlayerLevelData;
            levelsStatus = data.levelsStatus;
            for(int i=0;i<levelsStatus.Count;i++)
            {
                if (levelsStatus[i])
                {
                    count++;
                }
                if(count == 20)
                {
                    result.Add(count);
                    count = 0;
                }
            }
            if(count != 0)
                result.Add(count);
            stream.Close();
            return result;
        }
        else
        {
            PlayerLevelData data = new PlayerLevelData();
            List<bool> li = new List<bool>(new bool[1]);
            li[0] = true;
            data.levelsStatus = li;
            SaveLevelData(data);
        }
        return result;
    }
}
