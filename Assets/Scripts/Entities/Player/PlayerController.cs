using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.Input;
using NUnit.Framework;

public class PlayerController : MonoBehaviour
{
    float _nextFootstepAudio = 0.0f;
    private Rigidbody2D _rb;
    [SerializeField] float movementSpeed = 10.0f;
    [SerializeField] SpriteRenderer _characterBody;
    [SerializeField] ParticleSystem _overdriveParticles;
    [SerializeField] private Animator _animator;
    [SerializeField] AudioClip _footstep;
    [SerializeField] PlayerHealthSystem _healthSystem;

    private Material playerMaterial;
    private static readonly int OverdriveAmount =
    Shader.PropertyToID("_OverdriveAmount");
    [SerializeField] private SpriteRenderer playerSprite;
    bool isOverdrive;
    Color color;
    private bool isDead; // I guess we can keep this for the animator


    private readonly List<GameObject> nearbyEnemies = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        _healthSystem = GetComponent<PlayerHealthSystem>();
    }

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
        color = _characterBody.color;
        isDead = false;
        _healthSystem.OnPlayerDeath += PlayDeathAnimation;
        playerMaterial = playerSprite.material;
        playerMaterial.SetFloat(OverdriveAmount, 0f);
        isOverdrive = false;
        _overdriveParticles.Stop();
        _healthSystem.OnHealthStateChanged += HandleHealthStateChanged;
    }

    void Update()
    {


        if (!isDead)
        {
            HandlePlayerMovement();
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
        }

       
   

    }

    public void SetOverdrive(float amount)
    {
        playerMaterial.SetFloat(OverdriveAmount, amount);
    }




    private void HandlePlayerMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        movement = Vector2.ClampMagnitude(movement, 1.0f);
        _rb.linearVelocity = movement * movementSpeed;

        bool characterIsWalking = movement.magnitude > 0.0f;
        _animator.SetBool("isWalking", characterIsWalking);
        if (characterIsWalking)
        {
            HandleWalkingSounds();
        }

        bool flipSprite = movement.x < 0.0f;
        _characterBody.flipX = flipSprite;
    }

    IEnumerator alphaLerpingFunction(float endValue, float duration)
    {
        float time = 0;
        float startValue = color.a;

        while(time < duration)
        {
            
            time += Time.deltaTime;
            if(time > 1.0)
            {
                color.a = Mathf.Lerp(startValue, endValue, (time - 1.0f) / duration);
                _characterBody.color = color;
            }
            
            yield return null;
        }
        color.a = endValue;
        _characterBody.color = color;
    }

    public void HandleWalkingSounds()
    {
        if(Time.time >= _nextFootstepAudio)
        {
            
            AudioManager.Instance.PlayAudio(_footstep, AudioManager.SoundType.SFX, 1f, false);
            float audioFrequency = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / 2f;
            _nextFootstepAudio = Time.time + audioFrequency;
        }
    }

    void PlayDeathAnimation()
    {
        isDead = true;
        _animator.SetBool("isDead", isDead);
        StartCoroutine(alphaLerpingFunction(0.5f, 4.5f));
    }

    public bool getIsDead()
    {
        return isDead;
    }

    public void HandleHealthStateChanged(HealthState newState)
    {
        if(newState == HealthState.Underdrive) movementSpeed = 5.0f;
        else if(newState == HealthState.Overdrive)
        {
            movementSpeed = 12.5f;
            SetOverdrive(0.7f);
            _overdriveParticles.Play();
            isOverdrive = true;
        }
        else if(newState == HealthState.Normal)
        {
            isOverdrive = false;
            SetOverdrive(0.0f);
            _overdriveParticles.Stop();
            movementSpeed = 10.0f;
        }

    }
}
