using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image playAgainPanel;
    [SerializeField] Image gameWinPanel;
    [SerializeField] TMP_Text levelNumber;
    [SerializeField] TMP_Text winPanelNextButtonText;
    [SerializeField] Button startButton;
    LevelInfo levelInfo;
    TMP_Text tmpText;

    private void Awake()
    {
        //Application.targetFrameRate = 120;
    }
    private void Start()
    {
        levelInfo = FindObjectOfType<LevelInfo>();
        levelNumber.text = "LEVEL " + levelInfo.level.levelNumber.ToString();
    }

    public void ShowPlayAgainPanel()
    {
        foreach(GameObject att in levelInfo.level.instantiatedAttachments)
        {
            att.GetComponent<Attachment>().isDraggable = false;
        }
        tmpText = playAgainPanel.gameObject.GetComponentInChildren<TMP_Text>();
        tmpText.text = "GAME OVER\r\n\r\nTiles should not stop or go outside the grid.";
        playAgainPanel.gameObject.SetActive(true);
    }
    public void ClosePlayAgainPanel()
    {
        playAgainPanel.gameObject.SetActive(false);
    }

    public void ShowGameWinPanel()
    {
        tmpText = gameWinPanel.gameObject.GetComponentInChildren<TMP_Text>();
        if (levelInfo.level.levelNumber.ToString() == "20")
        {
            tmpText.text = "YOU WIN!\r\n\r\nYou have completed all levels.\r\nThank you for Playing";
            winPanelNextButtonText.text = "EXIT";
        }
        else
            tmpText.text = "YOU WIN!\r\n\r\nClick next to select the unlocked level.";
        gameWinPanel.gameObject.SetActive(true);
    }
    public void CloseGameWinPanel()
    {
        gameWinPanel.gameObject.SetActive(false);
    }

    public void DisableStartButton()
    {
        startButton.enabled = false;
    }
}
