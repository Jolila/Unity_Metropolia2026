using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VideoSettingsManager : MonoBehaviour
{

    [SerializeField] private Volume globalVolume;
    private FilmGrain filmGrain;

    private bool filmGrainEnabled;
    private bool fullscreen;
    private bool useHighResolution;

    public void SetFilmGrain(bool enabled)
    {
        filmGrainEnabled = enabled;
        filmGrain.active = enabled;
        SaveSettings();
    }

    public void Set1280x960() {
        Screen.SetResolution(1280, 960, Screen.fullScreenMode);
        useHighResolution = false;
    }

    public void Set1920x1440() {
        Screen.SetResolution(1920, 1440, Screen.fullScreenMode);
        useHighResolution = true;
    }

    public void SetWindowed() { Screen.fullScreenMode = FullScreenMode.Windowed; }

    public void SetFullscreen() { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; }


    private void Awake()
    {
        globalVolume.profile.TryGet(out filmGrain);

        LoadSettings();
        ApplySettings();
    }

    public void ApplySettings()
    {
        if (useHighResolution) Set1920x1440();
        else Set1280x960();

        if (fullscreen)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;

        filmGrain.active = filmGrainEnabled;
    }

    private void LoadSettings()
    {
        useHighResolution = PlayerPrefs.GetInt("UseHighResolution", 1) == 1;
        fullscreen = PlayerPrefs.GetInt("VideoFullscreen", 0) == 1;
        filmGrainEnabled = PlayerPrefs.GetInt("VideoFilmGrain", 1) == 1;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("UseHighResolution", useHighResolution ? 1 : 0);
        PlayerPrefs.SetInt("VideoFullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.SetInt("VideoFilmGrain", filmGrainEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
