using UnityEngine;

public class PlayerStaffController : MonoBehaviour
{

    [SerializeField] Projectile _projectile;
    [SerializeField] AudioClip _shootSound;
    [SerializeField] Transform _tip;
    [SerializeField] float _fireRate;
    Vector2 _lookDirection;
    float _nextFireTime;
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f) return;
        SetLookDirection();
        RotateStaff();
        if(Input.GetButton("Fire1") && Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + 1f / _fireRate;
            Shoot();
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


}
