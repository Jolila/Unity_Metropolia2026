using UnityEngine;

public class RewardSystem : MonoBehaviour
{

    public static RewardSystem Instance { get; private set; }

    public bool IsActive => rewardWindowRemaining > 0f;
    private float StartingRewardWindow = 10f;
    private float RewardWindowDuration;

    private float rewardWindowRemaining;



    private GameRound rewardRound;
    private float EnemyPollRate = 0.25f;
    private float enemyCheckTimer;

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
            if (EnemyPoolManager.Instance.PoolHasActiveEnemies(e)) return false;
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
        }

    }

    private void ResolveReward()
    {
        float completionTime = rewardWindowRemaining;
        rewardWindowRemaining = 0f;
        Debug.Log("Player has received a reward!");
    }


  


}
