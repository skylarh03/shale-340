using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioOptions : MonoBehaviour
{
    public void UpdateMusicVolume(float volume)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Global Music Volume", volume);
    }

    public void UpdateSfxVolume(float volume)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Global SFX Volume", volume);
    }

    public void UpdateEnvironmentVolume(float volume)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Global Environment Volume", volume);
    }

    public void LoadTitleScreen()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Button Click");
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
    }
}
