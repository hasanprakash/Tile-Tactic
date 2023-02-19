using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerLevelData
{
    public List<bool> levelsStatus;

    public PlayerLevelData(List<bool> arr)
    {
        levelsStatus = arr;
    }
    public PlayerLevelData() { }
}
