using UnityEngine;
using System.Collections;

public class BloodDroplet : MonoBehaviour
{

    private float killTimer = 5f;
    [SerializeField] private BloodDropletTier tier;

    public BloodDropletTier Tier => tier;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnable()
    {
        StartCoroutine(Lifetime());
    }

   

    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(killTimer);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BloodSystem.Instance.CollectBlood(tier);
            gameObject.SetActive(false);

        }
        
    }
}
