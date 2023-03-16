using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    AudioManager audioManager;
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
    private void Start()
    {
        audioManager = AudioManager.instance;
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 90;
    }

    // MENU SCENE
    public void ToLevelSelection()
    {
        if (audioManager == null) audioManager = AudioManager.instance;
        audioManager.Play("ButtonClick");
        SceneManager.LoadScene("LevelScene");
    }
    public void ToInstuction()
    {
        if (audioManager == null) audioManager = AudioManager.instance;
        audioManager.Play("ButtonClick");
        SceneManager.LoadScene("InstructionScene");
    }
    public void AppExit()
    {
        if (audioManager == null) audioManager = AudioManager.instance;
        audioManager.Play("ButtonClick");
        Application.Quit();
    }
    
    // LEVEL SCENE
    public void OpenLevel()
    {
        if (audioManager == null) audioManager = AudioManager.instance;
        audioManager.Play("ButtonClick");
        SceneManager.LoadScene("GameScene");
    }
    public void BackToMainMenu()
    {
        if (audioManager == null) audioManager = AudioManager.instance;
        audioManager.Play("ButtonClick");
        SceneManager.LoadScene("MenuScene");
    }

    // MAIN GAME SCENE
    public void PlayAgain()
    {
        if (audioManager == null) audioManager = AudioManager.instance;
        audioManager.Play("ButtonClick");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ExitToLevelScene()
    {
        if (audioManager == null) audioManager = AudioManager.instance;
        audioManager.Play("ButtonClick");
        SceneManager.LoadScene("LevelScene");
    }
}
