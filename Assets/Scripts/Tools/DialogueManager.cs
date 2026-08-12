using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] CanvasGroup dialogueScreen;


    private void Awake()
    {
        CanvasGroupSetState(dialogueScreen, false);
    }

    public void StartIntroDialogue()
    {
        CanvasGroupSetState(dialogueScreen, true);
    }

    public void FinishDialogue()
    {
        CanvasGroupSetState(dialogueScreen, false);

        GameManager.Instance.OnNewGameRequested();
    }

    private void CanvasGroupSetState(CanvasGroup canvasGroup, bool state)
    {
        canvasGroup.alpha = state ? 1f : 0f;
        canvasGroup.interactable = state;
        canvasGroup.blocksRaycasts = state;
    }
}
