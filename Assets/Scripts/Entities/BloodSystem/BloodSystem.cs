using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum BloodDropletTier
{
    Small, Medium, Large
}

[Serializable]
public class BloodDropWeight
{
    public BloodDropletTier tier;

    [Min(0f)]
    public float weight;
}



public class BloodSystem : MonoBehaviour
{

    private static BloodSystem _instance;
    public static BloodSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BloodSystem>();
                if (_instance == null)
                {
                    Debug.Log(" Error : no blood system instance");
                }
            }
            return _instance;
        }
    }


    public Dictionary<BloodDropletTier, float> ContributionToTotalBloodCount = 
        new Dictionary<BloodDropletTier, float> 
    {
            {BloodDropletTier.Small, 0.5f },
            {BloodDropletTier.Medium, 1.0f },
            {BloodDropletTier.Large, 2.5f }
    };

    public Dictionary<BloodDropletTier, float> ContributionToPlayerHealthIncrease =
        new Dictionary<BloodDropletTier, float>
        {
        { BloodDropletTier.Small, 1.5f},
        { BloodDropletTier.Medium, 4.5f},
        { BloodDropletTier.Large, 9.0f},
        };


    float totalBloodCollected = 0f;
    float bloodMoonVisibleQuota = 10f; // dummy test values for POC blood collecting
    float bloodMoonFullQuota = 25f;

    public float TotalBloodCollected => totalBloodCollected;
    public float BloodMoonVisibleQuota => bloodMoonVisibleQuota;
    public float BloodMoonFullQuota => bloodMoonFullQuota;

    [SerializeField] ObjectPool SmallDropletPool;
    [SerializeField] ObjectPool MediumDropletPool;
    [SerializeField] ObjectPool LargeDropletPool;

    private float smallEnemyDropChance = 0.10f;
    private float mediumEnemyDropChance = 0.40f;
    private float largeEnemyDropChance = 0.80f;

    private BloodDropWeight[] smallEnemyWeights =
    {
        new BloodDropWeight
        {
            tier = BloodDropletTier.Small,
            weight = 1f
        }
    };


    private BloodDropWeight[] mediumEnemyWeights =
    {
        new BloodDropWeight
        {
            tier = BloodDropletTier.Small,
            weight = 0.6f
        },
        new BloodDropWeight
        {
            tier = BloodDropletTier.Medium,
            weight = 0.4f
        }
    };

    private BloodDropWeight[] largeEnemyWeights =
    {
        new BloodDropWeight
        {
            tier = BloodDropletTier.Small,
            weight = 0.3f
        },
        new BloodDropWeight
        {
            tier = BloodDropletTier.Medium,
            weight = 0.3f
        },
        new BloodDropWeight
        {
            tier = BloodDropletTier.Large,
            weight = 0.4f
        }
    };

    [SerializeField] BloodMoonController _moonController;
    public event Action OnBloodCollected;

    public void Initialize()
    {
        totalBloodCollected = 0f;
        _moonController.Initialize();
    }


    private float GetDropChance(BloodDropletTier maxTier)
    {
        return maxTier switch
        {
            BloodDropletTier.Small => smallEnemyDropChance,
            BloodDropletTier.Medium => mediumEnemyDropChance,
            BloodDropletTier.Large => largeEnemyDropChance,
            _ => 0f
        };
    }



    public void TrySpawnDroplet(
        BloodDropletTier maxTier, Vector3 position)
        {

        if (UnityEngine.Random.value > GetDropChance(maxTier)) return;
        BloodDropletTier tier = RollForDropletTier(maxTier);

        SpawnDroplet(tier, position);
        
    }


    private BloodDropletTier RollForDropletTier(BloodDropletTier maxTier)
    {

        BloodDropWeight[] activeWeights = maxTier switch
        {
            BloodDropletTier.Small => smallEnemyWeights,
            BloodDropletTier.Medium => mediumEnemyWeights,
            BloodDropletTier.Large => largeEnemyWeights
        };

        float totalWeight = 0f;

        foreach (BloodDropWeight entry in activeWeights) totalWeight += entry.weight;

        float roll = UnityEngine.Random.value * totalWeight;

        foreach (BloodDropWeight entry in activeWeights)
        {
            roll -= entry.weight;

            if (roll <= 0f)
                return entry.tier;
        }

        return BloodDropletTier.Small;


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnDroplet(BloodDropletTier tier, Vector3 pos)
    {

        GameObject droplet = GetDropletFromPool(tier);

        if (droplet == null)
            return;

        droplet.transform.position = pos;
        droplet.SetActive(true);
    }

    public void CollectBlood(BloodDropletTier tier)
    {
        totalBloodCollected +=
    ContributionToTotalBloodCount[tier];

        float healthIncrease =
            ContributionToPlayerHealthIncrease[tier];

        PlayerHealthSystem.Instance.Heal(
            healthIncrease);

        OnBloodCollected?.Invoke();
    }



    private GameObject GetDropletFromPool(BloodDropletTier tier)
    {
        return tier switch
        {
            BloodDropletTier.Small => SmallDropletPool.GetPooledObject(),
            BloodDropletTier.Medium => MediumDropletPool.GetPooledObject(),
            BloodDropletTier.Large => LargeDropletPool.GetPooledObject(),
            _ => null
        };
    }




}

