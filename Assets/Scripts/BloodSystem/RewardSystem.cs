using UnityEngine;

public class RewardSystem : MonoBehaviour
{

    private static RewardSystem _instance;
    public static RewardSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<RewardSystem>();
                if (_instance == null)
                {
                    Debug.Log(" Error : no reward system instance");
                }
            }
            return _instance;
        }
    }

    public bool IsActive => rewardWindowRemaining > 0f;
    private float StartingRewardWindow = 5f;
    private float RewardWindowDuration;

    private float rewardWindowRemaining;


  


    private GameRound rewardRound;
    private float EnemyPollRate = 0.25f;
    private float enemyCheckTimer;


    private float rewardSpawnTime = 1.5f;
    private float rewardSpawnTimer;

    private void OnEnable()
    {
        GameProgressionManager.Instance.OnCurrentRoundChanged += HandleRoundChanged;
        RewardWindowDuration = StartingRewardWindow;
    }

    private void OnDisable()
    {
        GameProgressionManager.Instance.OnCurrentRoundChanged -= HandleRoundChanged;
    }

    private void HandleRoundChanged(GameRound newRound)
    {
        rewardRound = (GameRound)((int)newRound - 1);
        RewardWindowDuration = StartingRewardWindow + 0.25f * (int)rewardRound; // I dunno, a quarter second per round?
        rewardWindowRemaining = RewardWindowDuration; 
        enemyCheckTimer = 0f;
    }






    private bool AllEnemiesAreKilled(GameRound previousRound)
    {
        // Query the previous round, and check if the pools are empty.

        if (!Rounds.Definitions.TryGetValue(rewardRound,
            out RoundDefinition definition)) return true;

        foreach(EnemyType e in definition.PoolFillPercent.Keys)
        {
            if (EnemyPoolManager.Instance.GetActiveCount(e) != 0)  return false;
        }

        return true;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsActive) return;

        rewardWindowRemaining -= Time.deltaTime;

        if (rewardWindowRemaining <= 0f)
        {
            rewardWindowRemaining = 0f;
            return;
        }

        enemyCheckTimer -= Time.deltaTime;
        if (enemyCheckTimer > 0f) return;

        enemyCheckTimer = EnemyPollRate;
        if(AllEnemiesAreKilled(rewardRound))
        {
            ResolveReward();
            rewardWindowRemaining = 0f;
        }


  

    }

    private void ResolveReward()
    {
        float completionTime = rewardWindowRemaining;
        float rewardScale = RewardWindowDuration / rewardWindowRemaining;
        BloodSystem.Instance.TrySpawnReward(rewardRound, rewardScale);

    }





}
