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


   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        StartCoroutine(RevealBloodMoon());
        BloodMoonGlowMask.padding = maskPaddingVector;
        FaintBloodMoonCanvasGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private IEnumerator RevealBloodMoon()
    {


        float elapsed = 0f;
        while (elapsed < 5f)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / 5f;

            FaintBloodMoonCanvasGroup.alpha =
                Mathf.Lerp(0f, 0.5f, t);

            yield return null;
        }

        FaintBloodMoonCanvasGroup.alpha = 0.5f;



        elapsed = 0f;
        while (elapsed < 10f)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / 10f;

            float topPadding = Mathf.Lerp(316f, 0f, t);
            BloodMoonGlowMask.padding = new Vector4(
                0f,
                0f,
                0f,
                topPadding
            );

            yield return null;
        }

        BloodMoonGlowMask.padding = new Vector4(
            0f,
            0f,
            0f,
            0f
        );
    }
}
