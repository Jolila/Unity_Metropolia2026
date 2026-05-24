using UnityEngine;

public class DamageDealer : MonoBehaviour
{

    [SerializeField] float _dps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        

        if(collision.gameObject.TryGetComponent(out EntityHealth entityHealth))
        {
            entityHealth.LoseHealth(Time.fixedDeltaTime * _dps);
            Debug.Log("Player hug");
        }

        
      
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
