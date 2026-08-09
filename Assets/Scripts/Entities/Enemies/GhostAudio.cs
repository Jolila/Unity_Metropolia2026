using UnityEngine;

public class GhostAudio : MonoBehaviour
{
    private EntityHealth health;

    private void Awake()
    {
        health = GetComponent<EntityHealth>();
    }

    private void OnEnable()
    {
        health.OnDeath += PlaySound;
    }

    private void OnDisable()
    {
        health.OnDeath -= PlaySound;
    }

    private void PlaySound()
    {
        AudioManager.Instance.PlayGhostDeath();
    }

}
