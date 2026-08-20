using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerStaffController : MonoBehaviour
{

    [SerializeField] Projectile _projectile;
    [SerializeField] FireRing _fireRing;
    [SerializeField] Transform _tip;
    [SerializeField] float _fireRate;
    float _ringRate = 0.5f;
    float _nextRingTime = 0f;

    [SerializeField] GameObject player;
    [SerializeField] ObjectPool _projectilePool;
    private Light2D staffLight;
    private PlayerHealthSystem playerHealth;
    private bool controlStaff;

    Vector2 _lookDirection;
    float _nextFireTime;
    private Camera mainCamera;
    [SerializeField] CircleCollider2D _staffCollider;


    float _defaultFireRate = 15f;
    float _overdriveFireRate = 25f;
    float _underdriveFirerate = 10f;

    float doubleShotChance = 0.2f;


    Vector2 defaultSpread = new Vector2(-7.5f, 7.5f);
    Vector2 underdriveSpread = new Vector2(-3f, 3f);
    Vector2 overdriveSpread = new Vector2(-11.5f, 11.5f);

    Color defaultStafflightColor = new Color(0.2f, 0.45f, 0.2f);
    Color overdriveStaffLightColor = new Color(0.6f, 0.15f, 0.15f);

    Vector2 currentSpread;


    // Update is called once per frame

    void Awake()
    {
        staffLight = GetComponentInChildren<Light2D>();
        mainCamera = Camera.main;
        _fireRate = _defaultFireRate;
        currentSpread = defaultSpread;
        _ringRate = 0.5f;
    }

    void Start()
    {
        playerHealth = player.GetComponent<PlayerHealthSystem>();

        playerHealth.OnPlayerDeath += StopStaffControl;
        playerHealth.OnHealthStateChanged += HandleHealthStateChanged;
        controlStaff = true;
        staffLight.intensity = 0f;
        staffLight.color = defaultStafflightColor;
    
    }

    public void OnGameStarted()
    {
      
        staffLight.intensity = 1.0f;
    }


    void Update()
    {

       

        if (Time.timeScale == 0f || !controlStaff) return;
        SetLookDirection();
        RotateStaff();

        if (GameManager.Instance.GetState() == GameState.Countdown) return;
        
        if (Input.GetButton("Fire1") && Time.time >= _nextFireTime
            && playerHealth.TryRequestProjectile())
        {
            _nextFireTime = Time.time + 1f / _fireRate;
            Shoot();
        }
        if(Input.GetButton("Fire2") && Time.time >= _nextRingTime && playerHealth.TryRequestFireRing() )
        {
            _nextRingTime = Time.time + 1f / _ringRate;
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
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _lookDirection = (mousePosition - (Vector2)transform.position).normalized;
    }

    void Shoot()
    {

        // add some amount of spread

        
        float randomizedSpread = Random.Range(currentSpread.x, currentSpread.y);
        float doubleSpread = currentSpread.x * 1.33f;
        //Vector2 spreadDirection = Quaternion.Euler(0, 0, Random.Range(-7.5f, 7.5f)) * _lookDirection;

      
        // roll for doubleshot
        if (Random.Range(0, 1.0f) > 1.0f - doubleShotChance)
        {
            GameObject go1 = _projectilePool.GetPooledObject();
            Projectile projectile1 = go1.GetComponent<Projectile>();
            projectile1.transform.position = _tip.position;
            projectile1.transform.rotation = Quaternion.identity;
            projectile1.InitializeProjectile(_tip.position,
                    Quaternion.Euler(0, 0, randomizedSpread - doubleSpread) * _lookDirection);
            
            GameObject go2 = _projectilePool.GetPooledObject();
            Projectile projectile2 = go2.GetComponent<Projectile>();
                projectile2.transform.position = _tip.position;
                projectile2.transform.rotation = Quaternion.identity;

                projectile2.InitializeProjectile(_tip.position,
                    Quaternion.Euler(0, 0, randomizedSpread + doubleSpread) * _lookDirection);
            
        }
        else
        {
            GameObject go1 = _projectilePool.GetPooledObject();
            Projectile projectile = go1.GetComponent<Projectile>();
            projectile.transform.position = _tip.position;
            projectile.transform.rotation = Quaternion.identity;
            projectile.InitializeProjectile(_tip.position,
                Quaternion.Euler(0,0, randomizedSpread) * _lookDirection);
        }
 
        AudioManager.Instance.PlayProjectileShoot();
        // Add recoil to player
        //Vector2 playerOffset = new Vector2(0.1f, 0.1f) * _lookDirection;
        //player.transform.position -= new Vector3(playerOffset.x, playerOffset.y, 0);
    }

    void UseFireRing()
    {
        FireRing newFireRing = Instantiate(_fireRing, transform.position, Quaternion.identity);
        AudioManager.Instance.PlayFireRing();
        newFireRing.InitializeFireAttack(transform);
    }

    void StopStaffControl()
    {
        controlStaff = false;
        staffLight.intensity = 0f;
    }

    public void HandleHealthStateChanged(HealthState newState)
    {
        if (newState == HealthState.Underdrive)
        {
            _fireRate = _underdriveFirerate;
            currentSpread = underdriveSpread;
            staffLight.intensity = 0.4f;
            doubleShotChance = 0.2f;
        }
        else if (newState == HealthState.Overdrive)
        {
            _fireRate = _overdriveFireRate;
            currentSpread = overdriveSpread;
            staffLight.color = overdriveStaffLightColor;
            doubleShotChance = 0.45f;
        }
        else if (newState == HealthState.Normal)
        {
            _fireRate = _defaultFireRate;
            currentSpread = defaultSpread;
            staffLight.color = defaultStafflightColor;
            staffLight.intensity = 1f;
            doubleShotChance = 0f;
        }
    }


}
