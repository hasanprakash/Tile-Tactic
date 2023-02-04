using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelSprite : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Level level;
    private LevelInfo levelInfo;
    private static bool spawned = false;
    private GameManager gameManager;
    private void Start()
    {
        levelInfo = FindObjectOfType<LevelInfo>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetLevel(Level level)
    {
        this.level = level;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        FindObjectOfType<LevelInfo>().level = level;
        gameManager.OpenLevel();
    }
}
