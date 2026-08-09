using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PostGameScreenController : MonoBehaviour
{

    [SerializeField] RectTransform[] _elements;
    [SerializeField] float _tweenEffectDuration = 0.1f;
    [SerializeField] float initialScale = 2.0f;
    float beat = 60f / 166;


    [SerializeField] AudioClip _popUpSound;


    public IEnumerator PlayAnimations()
    {
        foreach(var element in _elements)
        {
            yield return Pop(element);
            yield return new WaitForSecondsRealtime(beat);
        }
    }

    public void PrepareAnimation()
    {
        foreach (var element in _elements)
        {
            element.localScale = Vector3.zero;
        }
    }



    IEnumerator Pop(RectTransform rect)
    {


        float elapsed = 0f;

        while(elapsed < _tweenEffectDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = elapsed / _tweenEffectDuration;

            rect.localScale = Vector3.Lerp(
                Vector3.zero,
                Vector3.one,
                t);

            yield return null;
        }

        rect.localScale = Vector3.one;
    }



}
