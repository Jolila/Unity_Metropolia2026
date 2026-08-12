using System.Collections.Generic;
using UnityEngine;


public class RoundDefinition
{
    public Dictionary<EnemyType, float> PoolFillPercent = new()
    {
        { EnemyType.RatLeader, 0f },
        { EnemyType.RatFollower, 0f },
        { EnemyType.SlimeLeader, 0f },
        { EnemyType.SlimeFollower, 0f },
        { EnemyType.Bat, 0f },
        { EnemyType.ZombieLeader, 0f },
        { EnemyType.ZombieFollower, 0f },
        { EnemyType.Ghost, 0f }
    };
}

public enum GameRound
{
    Round0,
    Round1,
    Round2,
    Round3,
    Round4,
    Round5,
    Round6,
    Round7,
    Round8,
    Round9,
    Round10,
    Round11,
    Round12
}


public class GameProgressionManager : MonoBehaviour
{

    private readonly Dictionary<GameRound, RoundDefinition> rounds = new()
    {
        {


            GameRound.Round0,
            new RoundDefinition
            {
                PoolFillPercent =
                {
                    { EnemyType.RatLeader , 0.40f },
                    { EnemyType.RatFollower , 0.40f }

                    }
                }
            },

        {
            GameRound.Round1,
            new RoundDefinition
            {
                PoolFillPercent =
                {
                    { EnemyType.RatLeader , 0.40f },
                    { EnemyType.RatFollower , 0.40f },
                    { EnemyType.Bat , 0.20f }
                }
            }
        },

        {
            GameRound.Round2,
            new RoundDefinition
            {
                PoolFillPercent =
                {

                    {EnemyType.Bat , 0.3f},
                    {EnemyType.SlimeLeader , 0.4f },
                    {EnemyType.SlimeFollower , 0.4f },
                    

                }
            }
        }
    };




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

    public RoundDefinition CurrentRoundDefinition =>
    rounds[CurrentRound];

    public void Initialize()
    {
        CurrentRound = GameRound.Round0;
    }

    public void SetRound(GameRound round)
    {
        CurrentRound = round;
    }


}
