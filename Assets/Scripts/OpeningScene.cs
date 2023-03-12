using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningScene : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip clip;
    void Awake()
    {
        clip.AddEvent(new AnimationEvent()
        {
            time = clip.length,
            functionName = "OnOpeningSceneCompletes"
        });
    }

    public void OnOpeningSceneCompletes()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
