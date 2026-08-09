using UnityEngine;

public class BloodDroplet : MonoBehaviour
{

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BloodSystem.Instance.CollectBlood(tier);
            gameObject.SetActive(false);

        }
        
    }
}
