using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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
        { BloodDropletTier.Small, 1.0f},
        { BloodDropletTier.Medium, 2.0f},
        { BloodDropletTier.Large, 3.0f},
        };


    float totalBloodCollected = 0f;
    float bloodMoonVisibleQuota = 200f; // dummy test values for POC blood collecting
    float bloodMoonFullQuota = 1200f;

    [SerializeField] Light2D GlobalLight;

    public float TotalBloodCollected => totalBloodCollected;
    public float BloodMoonVisibleQuota => bloodMoonVisibleQuota;
    public float BloodMoonFullQuota => bloodMoonFullQuota;

    [SerializeField] ObjectPool SmallDropletPool;
    [SerializeField] ObjectPool MediumDropletPool;
    [SerializeField] ObjectPool LargeDropletPool;

    [SerializeField] GameObject smallDroplet;
    [SerializeField] GameObject mediumDroplet;
    [SerializeField] GameObject largeDroplet;

    private float smallEnemyDropChance = 0.155f;
    private float mediumEnemyDropChance = 0.33f;
    private float largeEnemyDropChance = 0.50f;

    [SerializeField] BloodMoonController _moonController;
    public event Action OnBloodCollected;

    public void Initialize()
    {
        totalBloodCollected = 0f;
        _moonController.Initialize();
        GameProgressionManager.Instance.Initialize();
    }


    private float GetDropChance(BloodDropletTier maxTier)
    {
        float baseChance = maxTier switch
        {
            BloodDropletTier.Small => smallEnemyDropChance,
            BloodDropletTier.Medium => mediumEnemyDropChance,
            BloodDropletTier.Large => largeEnemyDropChance,
            _ => 0f
        };
        return baseChance + GetRoundDropChanceBonus();
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
            BloodDropletTier.Small => GetSmallEnemyWeights(),
            BloodDropletTier.Medium => GetMediumEnemyWeights(),
            BloodDropletTier.Large => GetLargeEnemyWeights()
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

    private const float MaxRoundDropChanceBonus = 0.05f;
    private const float MaxDropWeightTransfer = 0.05f;

    private float GetRoundDropChanceBonus()
    {
        float normalizedRound =
      (float)GameProgressionManager.Instance.CurrentRound /
        12f; // stupid hard coded shit, but working with enums was not aseasy as I had hoped lol

        return normalizedRound * MaxRoundDropChanceBonus;
    }

    private float GetRoundProgression()
    {
        return (int)GameProgressionManager.Instance.CurrentRound / 12f;
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
        if (droplet == null) return;
        // Honestly what the fuck is Visual Studio trying to do with this formatting


        
        Vector3 playerPos = GameManager.Instance.GetPlayerReference().transform.position;

        Vector3 sampledPos = Vector3.Lerp(pos, playerPos,
            UnityEngine.Random.Range(0.33f, 0.8f));


        Vector2 offset = new Vector2(sampledPos.x, sampledPos.y) + UnityEngine.Random.insideUnitCircle * 1.0f;
        droplet.transform.position = new Vector3(offset.x, offset.y, pos.z);
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

        float redness = Mathf.Lerp(0f, 1.0f, TotalBloodCollected / BloodMoonFullQuota);
        Color newColor = new Color(
                1.0f,
                1.0f - redness,
                1.0f - redness
            );
        GlobalLight.color = newColor;

        OnBloodCollected?.Invoke();
    }

    private BloodDropWeight[] GetSmallEnemyWeights()
    {
        float transfer =
            MaxDropWeightTransfer * GetRoundProgression();

        return new[]
        {
        new BloodDropWeight
        {
            tier = BloodDropletTier.Small,
            weight = 0.8f - transfer
        },

        new BloodDropWeight
        {
            tier = BloodDropletTier.Medium,
            weight = 0.2f + transfer
        }
        };
    }

    private BloodDropWeight[] GetMediumEnemyWeights()
    {
        float transfer =
            MaxDropWeightTransfer * GetRoundProgression();

        return new[]
        {
        new BloodDropWeight
        {
            tier = BloodDropletTier.Small,
            weight = 0.6f - transfer
        },

        new BloodDropWeight
        {
            tier = BloodDropletTier.Medium,
            weight = 0.4f + transfer
        }
    };
    }

    private BloodDropWeight[] GetLargeEnemyWeights()
    {
        float transfer =
            MaxDropWeightTransfer * GetRoundProgression();

        return new[]
        {

        new BloodDropWeight
        {
            tier = BloodDropletTier.Medium,
            weight = 0.4f - transfer
        },

        new BloodDropWeight
        {
            tier = BloodDropletTier.Large,
            weight = 0.4f + transfer
        }
    };
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

    public void TrySpawnReward(GameRound round, float percentage)
    {
        float percent = (int)Mathf.Round(percentage * 100f);


        // get the percentage. A better ratio would be : for 25% a big one, for 5% medium one, 1% small one.

        int bigs = (int)(percent / 25f);
        float percent2 = percent - bigs * 25;
        int meds = (int)(percent2 / 10f);
        float percent3 = percent2 - meds * 10f;
        int smalls = (int)(percent3);


        Vector2 playerpos = 
            new Vector2(GameManager.Instance.GetPlayerReference().transform.position.x,
            GameManager.Instance.GetPlayerReference().transform.position.y);

        for (int i = 0; i < bigs; ++i)
        {
            Vector2 pos = playerpos + UnityEngine.Random.insideUnitCircle * 2.5f;
            Instantiate(largeDroplet, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        }

        for (int i = 0; i < meds; ++i)
        {
            Vector2 pos = playerpos + UnityEngine.Random.insideUnitCircle * 2.5f;
            Instantiate(mediumDroplet, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        }

        for (int i = 0; i < smalls; ++i)
        {
            Vector2 pos = playerpos + UnityEngine.Random.insideUnitCircle * 2.5f;
            Instantiate(smallDroplet, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        }

    }




}

