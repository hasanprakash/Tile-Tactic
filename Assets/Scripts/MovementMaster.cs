using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MovementMaster : MonoBehaviour
{
    private LevelInfo levelInfo;
    private Level level;
    List<GameObject> attachments;
    HeroMovement heroMovement;
    TileMovement tileMovement;
    private bool isGameStarted = false;
    private int turnNum = 0; // 0 -> tile, 1 -> attachment; // here hero moves first
    private float timeToMove = 0.25f;
    public bool isHeroMovement { set; private get; }
    public bool isTileMovement { set; private get; }
    private UIManager uiManager;
    GridManager gridManager;

    private void Awake()
    {
        levelInfo = FindObjectOfType<LevelInfo>();
        level = levelInfo.level;
    }
    void Start()
    {
        uiManager = GetComponent<UIManager>();
        gridManager = FindObjectOfType<GridManager>();
        isTileMovement = true;
        isHeroMovement = false;
    }
    public void StartGame()
    {
        uiManager.ClosePlayAgainPanel();
        attachments = level.instantiatedAttachments;
        StopGameRoutine(gameObject);
        isGameStarted = true;
        Debug.Log("Starting Game");
        foreach (GameObject att in attachments)
        {
            Attachment attachment;
            att.gameObject.TryGetComponent<Attachment>(out attachment);
            att.gameObject.TryGetComponent<TileMovement>(out tileMovement);
            if(attachment)
            {
                attachment.RestoreToStartPosition();
                attachment.isDraggable = false;
            }
            if (tileMovement) tileMovement.MovementConfiguration();
        }
        level.instantiatedHero.GetComponent<HeroMovement>().EnableCollider();
        StartGameRoutine(true, true);
    }
    public void GameFailed()
    {
        uiManager.ShowPlayAgainPanel();
        isGameStarted = false;
        isHeroMovement = false;
        isTileMovement = false;

        level.instantiatedHero.GetComponent<HeroMovement>().DisableCollider();
        StopGameRoutine(gameObject);
    }
    public void GameWin()
    {
        isGameStarted = false;
        isHeroMovement = false;
        isTileMovement = false;

        SaveSystem.UnlockLevel(level.levelNumber + 1);
        
        uiManager.ShowGameWinPanel();

        level.instantiatedHero.GetComponent<HeroMovement>().DisableCollider();
        StopGameRoutine(gameObject);
    }
    void StartMovements()
    {
        //heroMovement = level.instantiatedHero.GetComponent<HeroMovement>();
        if (turnNum == 0)
        {
            if (isHeroMovement && heroMovement != null)
                heroMovement.HeroMove();
            turnNum = 1;
        }
        else if (turnNum == 1)
        {
            foreach (GameObject attachment in attachments)
            {
                attachment.gameObject.TryGetComponent<TileMovement>(out tileMovement);

                if (gridManager.IsPositionInsideAttachmentGrid(attachment.transform.position)) continue;
                if (isTileMovement && tileMovement != null)
                    tileMovement.AttachmentMove();
            }
            turnNum = 0;
        }
    }

    public void StartGameRoutine(bool HeroMovementStatus, bool TileMovementStatus)
    {
        StopGameRoutine(gameObject);
        heroMovement= level.instantiatedHero.GetComponent<HeroMovement>();
        isHeroMovement = HeroMovementStatus;
        isTileMovement = TileMovementStatus;
        InvokeRepeating("StartMovements", timeToMove, timeToMove + 0.3f);
    }
    public void StopGameRoutine(GameObject gameObject)
    {
        gameObject.TryGetComponent<HeroMovement>(out heroMovement);
        gameObject.TryGetComponent<TileMovement>(out tileMovement);
        if(heroMovement) heroMovement.StopMovement();
        if(tileMovement) tileMovement.StopMovement();
        CancelInvoke();
    }
}
