using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    EntityHealth _playerHealth;
    private void OnEnable()
    {
        _playerHealth = GetComponent<EntityHealth>();
        _playerHealth._isDead = false;
        _playerHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        _playerHealth.OnDeath -= HandleDeath;
    }

    public void HandleDeath()
    {
        GameManager.Instance.StopGameTimer();
        GameManager.Instance.SetGameEndsNow();
        StartCoroutine(WaitAndExecute());
    }

    IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(3f);
        GameManager.Instance.GameOver();
    }
}
