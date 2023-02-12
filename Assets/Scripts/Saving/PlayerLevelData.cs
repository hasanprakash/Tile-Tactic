using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerLevelData
{
    public bool[] levelsStatus;

    public PlayerLevelData(bool[] arr)
    {
        levelsStatus = arr;
    }
    public PlayerLevelData() { }
}
