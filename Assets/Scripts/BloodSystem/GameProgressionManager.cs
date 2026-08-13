using System;
using System.Collections.Generic;
using UnityEngine;






public class GameProgressionManager : MonoBehaviour
{

    private float CanonEvent1 => BloodSystem.Instance.BloodMoonVisibleQuota;
    private float CanonEvent2 => BloodSystem.Instance.BloodMoonFullQuota;

    private const float CanonEvent1RoundProgressionExponent = 2f;
    private const float CanonEvent2RoundProgressionExponent = 2f; // This needs tweaking, might be as high as 4, but likely needs a linear component.

    public Action <GameRound> OnCurrentRoundChanged;

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
        DebugRoundThresholds();
        CurrentRound = GameRound.Round0;
        UpdateNextRoundThreshold();
        
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

    private void DebugRoundThresholds()
    {
        Debug.Log("========== ROUND BLOOD THRESHOLDS ==========");

        for (int round = 0; round < roundBloodThresholds.Length; round++)
        {
            Debug.Log(
                $"Round {round}: " +
                $"{roundBloodThresholds[round]:F2} blood"
            );
        }

        Debug.Log("============================================");
    }



    public void SetRound(GameRound round)
    {
        CurrentRound = round;
    }

    private void HandleBloodCollected()
    {
        if (BloodSystem.Instance.TotalBloodCollected <
       nextRoundBloodThreshold)
        {
            return;
        }

        CurrentRound++;

        UpdateNextRoundThreshold();
        Debug.Log("Updated round to " + CurrentRound);
        OnCurrentRoundChanged?.Invoke(
            CurrentRound
        );
    }




  


}
