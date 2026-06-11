using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerStaffController : MonoBehaviour
{

    [SerializeField] Projectile _projectile;
    [SerializeField] FireRing _fireRing;
    [SerializeField] AudioClip _shootSound;
    [SerializeField] AudioClip _fireRingSound;
    [SerializeField] Transform _tip;
    [SerializeField] float _fireRate;
    [SerializeField] float _fireRingRate;
    [SerializeField] GameObject player;
    private Light2D staffLight;
    private EntityHealth playerHealth;
    private bool controlStaff;
    float _nextFireRingTime;
    Vector2 _lookDirection;
    float _nextFireTime;
    // Update is called once per frame

    void Awake()
    {
        staffLight = GetComponentInChildren<Light2D>();
    }

    void Start()
    {
        playerHealth = player.GetComponent<EntityHealth>();
        playerHealth.OnDeath += StopStaffControl;
        controlStaff = true;

    }
    void Update()
    {
        if (Time.timeScale == 0f || !controlStaff) return;
        SetLookDirection();
        RotateStaff();
        if(Input.GetButton("Fire1") && Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + 1f / _fireRate;
            Shoot();
        }
        if(Input.GetButton("Fire2") && Time.time >= _nextFireRingTime)
        {
            _nextFireRingTime = Time.time + 1f / _fireRingRate;
            UseFireRing();
        }
    }

    void RotateStaff()
    {
        float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void SetLookDirection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _lookDirection = (mousePosition - (Vector2)transform.position).normalized;
    }

    void Shoot()
    {
        Projectile newProjectile = Instantiate(_projectile, _tip.position, Quaternion.identity);
        AudioManager.Instance.PlayAudio(_shootSound, AudioManager.SoundType.SFX, 0.4f, false);
        newProjectile.InitializeProjectile(_lookDirection);
    }

    void UseFireRing()
    {
        FireRing newFireRing = Instantiate(_fireRing, transform.position, Quaternion.identity);
        AudioManager.Instance.PlayAudio(_fireRingSound, AudioManager.SoundType.SFX, 0.3f, false);
        newFireRing.InitializeFireAttack(transform);
    }

    void StopStaffControl()
    {
        controlStaff = false;
        staffLight.intensity = 0f;
    }


}
