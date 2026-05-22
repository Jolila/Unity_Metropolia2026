using UnityEngine;

public class PlayerStaffController : MonoBehaviour
{

    [SerializeField] float _fireRate;
    float _nextFireTime;
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f) return;
        RotateStaff();
        if(Input.GetButton("Fire1") && Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + 1f / _fireRate;
            Shoot();
        }
    }

    void RotateStaff()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 lookDirection = (mousePosition - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Shoot()
    {
        Debug.Log("BANGARANGA");
    }


}
