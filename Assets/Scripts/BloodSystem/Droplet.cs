using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class BloodDroplet : MonoBehaviour
{

    private float killTimer = 6f;
    [SerializeField] private BloodDropletTier tier;
    [SerializeField] SpriteRenderer _renderer;
    private Material dropletMaterial;
    private static readonly int GlowIntensityID =
    Shader.PropertyToID("_Glow_Intensity");
    [SerializeField] Light2D _light;

    private MaterialPropertyBlock propertyBlock;

    public BloodDropletTier Tier => tier;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dropletMaterial = _renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        SetGlow(1f);
        StartCoroutine(Lifetime());
        
    }

   private void SetGlow(float amount)
    {
        
        _renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(GlowIntensityID, amount);
        _renderer.SetPropertyBlock(propertyBlock);
        _light.intensity = amount * 0.5f;
    }

    IEnumerator Lifetime()
    {

        float start = 0f;
        while(start < killTimer)
        {
            float t = start / killTimer;
            SetGlow(1 - 0.5f*t);

            start += Time.deltaTime;
            yield return null;
        }
        SetGlow(0f);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BloodSystem.Instance.CollectBlood(tier);
            gameObject.SetActive(false);

        }
        
    }
}
