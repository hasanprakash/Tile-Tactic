using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            var gms = FindObjectsOfType<GameManager>();
            if (gms.Length > 1)
            {
                Destroy(gms[0].gameObject);
            }
        }
    }

    public void ToLevelSelection()
    {
        SceneManager.LoadScene("LevelScene");
    }
    public void ToInstuction()
    {
        SceneManager.LoadScene("InstructionScene");
    }
    public void AppExit()
    {
        Application.Quit();
    }
    
    // LEVEL SCENE
    public void OpenLevel()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    // MAIN GAME SCENE
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitToLevelScene()
    {
        SceneManager.LoadScene("LevelScene");
    }
}
