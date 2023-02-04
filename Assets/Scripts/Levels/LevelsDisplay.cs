using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelsDisplay : MonoBehaviour
{
    [SerializeField] Transform parent;
    [SerializeField] LevelSprite levelSpritePrefab;
    [SerializeField] List<Level> levels;
    [SerializeField] GameManager gameManager;

    private void Start()
    {
        for(int i=0;i<levels.Count;i++)
        {
            LevelSprite levelSpriteObject = Instantiate(levelSpritePrefab, parent);
            levelSpriteObject.name = $"Level {i + 1}";
            levelSpriteObject.GetComponentInChildren<TMP_Text>().text = (i+1).ToString();
            levelSpriteObject.SetLevel(levels[i]);
        }
    }
}
