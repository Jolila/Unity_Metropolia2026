using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerStaffController : MonoBehaviour
{

    [SerializeField] Projectile _projectile;
    [SerializeField] FireRing _fireRing;
    [SerializeField] Transform _tip;
    [SerializeField] float _fireRate;
    [SerializeField] float _fireRingRate;
    [SerializeField] GameObject player;
    [SerializeField] ObjectPool _projectilePool;
    private Light2D staffLight;
    private EntityHealth playerHealth;
    private bool controlStaff;
    float _nextFireRingTime;
    Vector2 _lookDirection;
    float _nextFireTime;
    private Camera mainCamera;

   
    // Update is called once per frame

    void Awake()
    {
        staffLight = GetComponentInChildren<Light2D>();
        mainCamera = Camera.main;
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

        if (GameManager.Instance.GetState() == GameState.Countdown) return;

        if (Input.GetButton("Fire1") && Time.time >= _nextFireTime)
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
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        _lookDirection = (mousePosition - (Vector2)transform.position).normalized;
    }

    void Shoot()
    {

        // add some amount of spread
        float randomizedSpread = Random.Range(-7f, 7f);
        float doubleSpread = 9f;
        //Vector2 spreadDirection = Quaternion.Euler(0, 0, Random.Range(-7.5f, 7.5f)) * _lookDirection;

      
        // roll for doubleshot
        if (Random.Range(0, 1.0f) > 0.8f)
        {
            GameObject go1 = _projectilePool.GetPooledObject();
            Projectile projectile1 = go1.GetComponent<Projectile>();
            projectile1.transform.position = _tip.position;
            projectile1.transform.rotation = Quaternion.identity;
            projectile1.InitializeProjectile(
                    Quaternion.Euler(0, 0, randomizedSpread - doubleSpread) * _lookDirection);
            
            GameObject go2 = _projectilePool.GetPooledObject();
            Projectile projectile2 = go2.GetComponent<Projectile>();
                projectile2.transform.position = _tip.position;
                projectile2.transform.rotation = Quaternion.identity;

                projectile2.InitializeProjectile(
                    Quaternion.Euler(0, 0, randomizedSpread + doubleSpread) * _lookDirection);
            
        }
        else
        {
            GameObject go1 = _projectilePool.GetPooledObject();
            Projectile projectile = go1.GetComponent<Projectile>();
            projectile.transform.position = _tip.position;
            projectile.transform.rotation = Quaternion.identity;
            projectile.InitializeProjectile(Quaternion.Euler(0,0, randomizedSpread) * _lookDirection);
        }
 
        AudioManager.Instance.PlayProjectileShoot();
        // Add recoil to player
        Vector2 playerOffset = new Vector2(0.1f, 0.1f) * _lookDirection;
        player.transform.position -= new Vector3(playerOffset.x, playerOffset.y, 0);
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


}
