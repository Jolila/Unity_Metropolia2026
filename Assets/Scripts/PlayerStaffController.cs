using UnityEngine;

public class PlayerStaffController : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        RotateStaff();

    }

    void RotateStaff()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 lookDirection = (mousePosition - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


}
