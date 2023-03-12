using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class LevelGroup : MonoBehaviour
{
    [SerializeField] List<TMP_Text> levelText;
    List<int> progressOfGroups = null;
    void Start()
    {
        progressOfGroups = SaveSystem.GetLevelProgress();
        for(int i=0;i<levelText.Count;i++)
        {
            if (i >= progressOfGroups.Count)
            {
                levelText[i].text = "0/20";
            }
            else
            {
                Debug.Log(progressOfGroups[i].ToString());
                levelText[i].text = progressOfGroups[i].ToString() + "/20";
            }
        }
    }
}
