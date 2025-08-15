using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
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
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(1.0f);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
