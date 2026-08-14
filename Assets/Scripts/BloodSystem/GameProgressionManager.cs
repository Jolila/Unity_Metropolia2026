using System;
using UnityEngine;


public class GameProgressionManager : MonoBehaviour
{

    private float CanonEvent1 => BloodSystem.Instance.BloodMoonVisibleQuota;
    private float CanonEvent2 => BloodSystem.Instance.BloodMoonFullQuota;

    private const float CanonEvent1RoundProgressionExponent = 2f;
    private const float CanonEvent2RoundProgressionExponent = 2f; // This needs tweaking, might be as high as 4, but likely needs a linear component.

    public Action <GameRound> OnCurrentRoundChanged;
    public Action OnBloodMoonFull;

    private static GameProgressionManager _instance;
    public static GameProgressionManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindAnyObjectByType<GameProgressionManager>();
            return _instance;
        }
    }


    public GameRound CurrentRound { get; private set; } = GameRound.Round0;
    private readonly float[] roundBloodThresholds = new float[13];
    private float nextRoundBloodThreshold;



    private void OnEnable()
    {
        BloodSystem.Instance.OnBloodCollected += HandleBloodCollected;
    }

    private void OnDisable()
    {
        BloodSystem.Instance.OnBloodCollected -= HandleBloodCollected;
    }

    public void Initialize()
    {
        CalculateRoundBloodThresholds();
        //DebugRoundThresholds();
        CurrentRound = GameRound.Round0;
        UpdateNextRoundThreshold();
        MusicProgressionManager.Instance.Initialize();
    }

    private void CalculateRoundBloodThresholds()
    {

        for (int round = 0; round <= 4; round++)
        {
            float normalizedRound = round / 4f;

            float curvedPosition = Mathf.Pow(
                normalizedRound,
                CanonEvent1RoundProgressionExponent
            );

            roundBloodThresholds[round] = Mathf.Lerp(
                0f,
                CanonEvent1,
                curvedPosition
            );
        }


        for (int round = 5; round <= 12; round++)
        {
            float normalizedRound = (round - 5) / 7f;

            float curvedPosition = Mathf.Pow(
                normalizedRound,
                CanonEvent2RoundProgressionExponent
            );

            roundBloodThresholds[round] = Mathf.Lerp(
                CanonEvent1,
                CanonEvent2,
                curvedPosition
            );
        }
    }

    private void UpdateNextRoundThreshold()
    {
        int nextround = (int)CurrentRound + 1;
        if(nextround > 12)
        {
            nextRoundBloodThreshold = BloodSystem.Instance.BloodMoonFullQuota;
            return;
        }
        nextRoundBloodThreshold = roundBloodThresholds[nextround];
    }



    public void SetRound(GameRound round)
    {
        CurrentRound = round;
    }

    private void HandleBloodCollected()
    {

        if (CurrentRound == GameRound.Boss) return;

        if (CurrentRound < GameRound.Round12)
        {
            TryAdvanceRound();
            return;
        }
        TryTriggerBoss();
    }
       

    

    private void TryAdvanceRound()
    {
        if (BloodSystem.Instance.TotalBloodCollected <
        nextRoundBloodThreshold)
        {
            return;
        }
        CurrentRound++;
        UpdateNextRoundThreshold();
        OnCurrentRoundChanged?.Invoke(CurrentRound);
    }

    private void TryTriggerBoss()
    {
        if (BloodSystem.Instance.TotalBloodCollected <
            BloodSystem.Instance.BloodMoonFullQuota)
        {
            return;
        }

        CurrentRound = GameRound.Boss;

        OnBloodMoonFull?.Invoke();
        Debug.Log("BloodMoon Is full...");
        OnCurrentRoundChanged?.Invoke(CurrentRound);
    }


}
