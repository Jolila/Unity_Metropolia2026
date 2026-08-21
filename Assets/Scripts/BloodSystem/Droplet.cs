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

    [SerializeField] private AnimationCurve vacuumSpeed;
    [SerializeField] private float maxDistance = 3.0f;
    [SerializeField] private float maxSpeed = 2.5f;
    private float stopHomingDistance = 4.0f;

    private MaterialPropertyBlock propertyBlock;

    public BloodDropletTier Tier => tier;
    bool closeToPlayer;
    
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
        if(closeToPlayer)
        {
            Vector3 helper = 
                GameManager.Instance.GetPlayerReference().transform.position - transform.position;

            float distance = helper.magnitude;
            if(distance > stopHomingDistance)
            {
                closeToPlayer = false;
                return;
            }

            float t = Mathf.Clamp01(distance / maxDistance);

            float speed = vacuumSpeed.Evaluate(t) * maxSpeed;
            transform.position += 
                helper.normalized
                * speed
                * Time.deltaTime;
        }
    }

    public void OnEnable()
    {
        SetGlow(1f);
        StartCoroutine(Lifetime());
        closeToPlayer = false;
    }

   private void SetGlow(float amount)
    {
        
        _renderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat(GlowIntensityID, amount * 0.1f);
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

        if(other.CompareTag("PlayerStaff"))
        {
            closeToPlayer = true;
        }
        
    }
}
