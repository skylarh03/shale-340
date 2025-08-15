using UnityEngine;
using UnityEngine.UI;

public class SliderInitializer : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _envSlider;

    private float _musicVolume;
    private float _sfxVolume;
    private float _envVolume;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName("Global Music Volume", out _musicVolume);
        _musicSlider.value = _musicVolume;
        
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName("Global SFX Volume", out _sfxVolume);
        _sfxSlider.value = _sfxVolume;
        
        FMODUnity.RuntimeManager.StudioSystem.getParameterByName("Global Environment Volume", out _envVolume);
        _envSlider.value = _envVolume;
    }
}
