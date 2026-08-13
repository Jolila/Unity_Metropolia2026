using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BloodMoonController : MonoBehaviour
{

    [SerializeField] private CanvasGroup FaintBloodMoonCanvasGroup;
    [SerializeField] private RectMask2D BloodMoonGlowMask;

    [SerializeField] private float faintMoonDuration = 5f;
    [SerializeField] private float revealDuration = 10f;


    Vector4 maskPaddingVector = new Vector4(
        0f,
        0f,
        0f,
        316f); // top

    // Update is called once per frame
    void Update()
    {
    
    }

    public void Initialize()
    {

        BloodMoonGlowMask.padding = maskPaddingVector;
        FaintBloodMoonCanvasGroup.alpha = 0f;

        BloodSystem.Instance.OnBloodCollected += UpdateBloodMoon;
    }

    private void UpdateBloodMoon()
    {

        float total = BloodSystem.Instance.TotalBloodCollected;
        float quotaFirst = BloodSystem.Instance.BloodMoonVisibleQuota;


        float FaintMoonAlpha =
            (BloodSystem.Instance.TotalBloodCollected / BloodSystem.Instance.BloodMoonVisibleQuota)
        *0.5f; // for normalizing the alpha to 0.5f;

        FaintBloodMoonCanvasGroup.alpha =
            Mathf.Lerp(0f, 0.5f, FaintMoonAlpha);

        if (total <= quotaFirst) return;

        float quotaSecond = BloodSystem.Instance.BloodMoonFullQuota;

        float paddingMod = BloodSystem.Instance.TotalBloodCollected / BloodSystem.Instance.BloodMoonFullQuota;
        paddingMod = Mathf.Min(1.0f, paddingMod);
        
        BloodMoonGlowMask.padding =
        new Vector4(
            0f,
            0f,
            0f,
            (316f - 316f * paddingMod));



    }


}
