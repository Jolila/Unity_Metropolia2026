using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Slider _mainSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _sfxSlider;
    void Start()
    {
        
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
