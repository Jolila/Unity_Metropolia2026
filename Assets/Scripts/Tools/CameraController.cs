using UnityEngine;
using System.Collections;
using static UnityEngine.Input;

public class CameraController : MonoBehaviour
{

    Transform playerTransform;
    Vector3 cameraBack = Vector3.back * 9f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
    {
        if (playerTransform == null)
            return;
        transform.position = playerTransform.position + cameraBack;
    }
}
