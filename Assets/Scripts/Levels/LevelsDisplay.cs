using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelsDisplay : MonoBehaviour
{
    [SerializeField] Transform parent;
    [SerializeField] LevelSprite levelSpritePrefab;
    [SerializeField] Image lockedLevelPrefab;
    [SerializeField] List<Level> levels;
    [SerializeField] GameManager gameManager;
    bool[] levelsStatus;

    private void Awake()
    {
        PlayerLevelData playerLevelData = SaveSystem.LoadLevel(levels.Count);
        levelsStatus = playerLevelData.levelsStatus;
        if(levelsStatus.Length != levels.Count) { Debug.Log("Got wrong data while retrieving level information"); }
    }

    private void Start()
    {
        for(int i=0;i<levels.Count;i++)
        {
            if (levelsStatus[i])
            {
                LevelSprite levelSpriteObject = Instantiate(levelSpritePrefab, parent);
                levelSpriteObject.name = $"Level {i + 1}";
                levelSpriteObject.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
                levelSpriteObject.SetLevel(levels[i]);
            }
            else
            {
                Instantiate(lockedLevelPrefab, parent);
            }
        }
    }
}
