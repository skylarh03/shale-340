using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1; // reset in case the player is coming from the pause menu
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/Mix/Exit House"); // reset mixer state to default snapshot
    }
    
    public void LoadGame()
    {
        StartCoroutine(SwitchSceneOnDelay("Environment"));
    }

    public void LoadAudioOptions()
    {
        StartCoroutine(SwitchSceneOnDelay("AudioOptions"));
    }

    public void Quit()
    {
        StartCoroutine(QuitGame());
    }

    IEnumerator SwitchSceneOnDelay(string nextScene)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Button Click");
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator QuitGame()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Button Click");
        yield return new WaitForSeconds(1.0f);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
