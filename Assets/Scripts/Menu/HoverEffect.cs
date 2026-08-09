using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler
{

    [SerializeField] float _hoverScaleIncrease = 1.3f;
    [SerializeField] float _clickScaleIncrease = 1.5f;
    [SerializeField] float _tweenEffectDuration = 0.1f;

    [SerializeField] Image _image;

    [SerializeField] AudioClip _hoverAudio;
    [SerializeField] AudioClip _clickAudio;
    [SerializeField] AudioClip _exitHoverAudio;

    private void Awake()
    {
        if(_image == null)
        {
            _image = GetComponent<Image>();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.one * _clickScaleIncrease;
        LeanTween.scale(gameObject, Vector2.one, _tweenEffectDuration).setIgnoreTimeScale(true);
        AudioManager.Instance.PlaySFX(_clickAudio, 1.0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
       
        LeanTween.scale(gameObject, Vector2.one * _hoverScaleIncrease,
            _tweenEffectDuration).setIgnoreTimeScale(true);
        AudioManager.Instance.PlaySFX(_hoverAudio, 1.0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector2.one, _tweenEffectDuration).setIgnoreTimeScale(true);
        AudioManager.Instance.PlaySFX(_exitHoverAudio, 1.0f);
    }

}
