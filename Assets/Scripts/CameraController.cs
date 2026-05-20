using UnityEngine;
using System.Collections;
using static UnityEngine.Input;

public class CameraController : MonoBehaviour
{

    Transform playerTransform;
    Vector3 previousPos;
    Vector3 initialCameraOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
        previousPos = playerTransform.position;
        initialCameraOffset = new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        previousPos = transform.position;
        // wonder if raycast deprojection is needed for querying mouse position in 2D game, but I guess it doesn't hurt
        Vector3 mouseScrPos = Input.mousePosition;
        Vector3 toWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScrPos.x, mouseScrPos.y, Camera.main.nearClipPlane));
        toWorldPos *= 0.1f;

        Vector3 targetPos = playerTransform.position + toWorldPos;
        

        transform.position = Vector3.Slerp(targetPos, previousPos, 0.1f);
        transform.position += initialCameraOffset;
    }
}
