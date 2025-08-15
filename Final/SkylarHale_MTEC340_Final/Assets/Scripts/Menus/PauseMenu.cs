using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Quitting game");
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Button Click");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
