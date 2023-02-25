using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image playAgainPanel;
    [SerializeField] Image gameWinPanel;
    [SerializeField] TMP_Text levelNumber;
    LevelInfo levelInfo;

    private void Awake()
    {
        Application.targetFrameRate = 120;
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
        playAgainPanel.gameObject.SetActive(true);
    }
    public void ClosePlayAgainPanel()
    {
        playAgainPanel.gameObject.SetActive(false);
    }

    public void ShowGameWinPanel()
    {
        gameWinPanel.gameObject.SetActive(true);
    }
    public void CloseGameWinPanel()
    {
        gameWinPanel.gameObject.SetActive(false);
    }
}
