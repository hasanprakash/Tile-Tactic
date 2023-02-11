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
    private float timeToMove = 0.5f;
    public bool isHeroMovement { set; private get; }
    public bool isTileMovement { set; private get; }
    private UIManager uiManager;

    private void Awake()
    {
        levelInfo = FindObjectOfType<LevelInfo>();
        level = levelInfo.level;
    }
    void Start()
    {
        uiManager = GetComponent<UIManager>();
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
            if(attachment) attachment.RestoreToStartPosition();
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
        uiManager.ShowGameWinPanel();
        isGameStarted = false;
        isHeroMovement = false;
        isTileMovement = false;

        level.instantiatedHero.GetComponent<HeroMovement>().DisableCollider();
        StopGameRoutine(gameObject);
    }
    void StartMovements()
    {
        heroMovement = level.instantiatedHero.GetComponent<HeroMovement>();
        foreach(GameObject attachment in attachments)
        {
            attachment.gameObject.TryGetComponent<TileMovement>(out tileMovement);

            if (isTileMovement && tileMovement != null)
                tileMovement.AttachmentMove();
        }
        if (isHeroMovement && heroMovement != null)
            heroMovement.HeroMove();
    }

    public void StartGameRoutine(bool HeroMovementStatus, bool TileMovementStatus)
    {
        StopGameRoutine(gameObject);
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) StartGameRoutine(true, false); // hero and tile
        if (Input.GetKeyDown(KeyCode.Y)) StopGameRoutine(gameObject);
    }
}
