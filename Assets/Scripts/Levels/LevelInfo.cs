using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInfo : MonoBehaviour
{
    [SerializeField] public Level level;
    public static LevelInfo levelInfoInstance;
    private void Awake()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if(levelInfoInstance != null && levelInfoInstance != this)
        {
            if(currentScene.name == "LevelScene")
            {
                Destroy(levelInfoInstance.gameObject);
                levelInfoInstance = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(levelInfoInstance);
        }
        else
        {
            levelInfoInstance = this;
            DontDestroyOnLoad(this);
        }
    }
}
