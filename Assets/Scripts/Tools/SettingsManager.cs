using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Slider _mainSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _sfxSlider;

    const float DEFAULT_MAIN_VOLUME = 0.5f;
    const float DEFAULT_MUSIC_VOLUME = 1f;
    const float DEFAULT_SFX_VOLUME = 1f;
    void Start()
    {
        _mainSlider.value = PlayerPrefs.GetFloat("Settings.MasterVolume", DEFAULT_MAIN_VOLUME);
        _musicSlider.value = PlayerPrefs.GetFloat("Settings.MusicVolume", DEFAULT_MUSIC_VOLUME);
        _sfxSlider.value = PlayerPrefs.GetFloat("Settings.SFXVolume", DEFAULT_SFX_VOLUME);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMainVolume(float newVol)
    {
        AudioManager.Instance.ChangeMasterVolume(newVol);
    }

    public void ChangeMusicVolume(float newVol)
    {
        AudioManager.Instance.ChangeMusicVolume(newVol);
    }

    public void ChangeSFXVolume(float newVol)
    {
        AudioManager.Instance.ChangeSFXVolume(newVol);
    }
}
