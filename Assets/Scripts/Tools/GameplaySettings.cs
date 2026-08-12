using UnityEngine;

public class GameplaySettings : MonoBehaviour
{

    private bool promptForIntroDialogue;
    private bool showBloodEffects;

    private void Awake()
    {
        LoadSettings();
        SaveSettings();
    }

    private void LoadSettings()
    {
        promptForIntroDialogue =
            PlayerPrefs.GetInt("PromptForIntroDialogue", 1) == 1;

        showBloodEffects =
            PlayerPrefs.GetInt("ShowBloodEffects", 1) == 1;
    }

    public void SetPromptForIntroDialogue(bool enabled)
    {
        promptForIntroDialogue = enabled;
        SaveSettings();
    }

    public void SetBloodEffects(bool enabled)
    {
        showBloodEffects = enabled;
        SaveSettings();
    }

    private void SaveSettings()
    {

        PlayerPrefs.SetInt(
            "PromptForIntroDialogue",
            promptForIntroDialogue ? 1 : 0
        );

        PlayerPrefs.SetInt(
            "ShowBloodEffects",
            showBloodEffects ? 1 : 0
        );

        PlayerPrefs.Save();
    }
}
