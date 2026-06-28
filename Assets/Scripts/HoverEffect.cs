using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverEffect : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler
{

    [SerializeField] float _hoverScaleIncrease = 1.1f;
    [SerializeField] float _clickScaleIncrease = 1.3f;
    [SerializeField] float _tweenEffectDuration = 0.1f;

    [SerializeField] Image _image;
    Color _onNotHoveredColor = new Color(0.3f, 0.3f, 0.3f, 0.8f); // roughly 0.3 
    Color _onHoverColor = new Color(1.0f, 0.05f, 0.05f, 1.0f);

    [SerializeField] AudioClip _hoverAudio;
    [SerializeField] AudioClip _clickAudio;
    [SerializeField] AudioClip _exitHoverAudio;

    private void Awake()
    {
        if(_image == null)
        {
            _image = GetComponent<Image>();
            _onNotHoveredColor = _image.color;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        transform.localScale = Vector2.one * _clickScaleIncrease;
        LeanTween.scale(gameObject, Vector2.one, _tweenEffectDuration).setIgnoreTimeScale(true);
        AudioManager.Instance.PlayAudio(_clickAudio, AudioManager.SoundType.SFX, 1.0f, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
       
        LeanTween.scale(gameObject, Vector2.one * _hoverScaleIncrease,
            _tweenEffectDuration).setIgnoreTimeScale(true);
        _image.color = _onHoverColor;
        AudioManager.Instance.PlayAudio(_hoverAudio, AudioManager.SoundType.SFX, 1.0f, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector2.one, _tweenEffectDuration).setIgnoreTimeScale(true);
        _image.color = _onNotHoveredColor;
        AudioManager.Instance.PlayAudio(_exitHoverAudio, AudioManager.SoundType.SFX, 0.6f, false);
    }

}
