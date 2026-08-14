using System.Collections.Generic;


// Data class for housing the data for enemy systems to react to the gameprogressionmanager events
public enum GameRound
{

    Round0,
    Round1,
    Round2,
    Round3,
    Round4,
    //canon event 2
    Round5,
    Round6,
    Round7,
    Round8,
    Round9,
    Round10,
    Round11,
    Round12,
    Boss
}

public class RoundDefinition
{
    public Dictionary<EnemyType, float> PoolFillPercent = new();
}


// This is the data class of the progression system. 
// I expect this class to change rapidly during balancing sprints, so keeping the data separate hopefully leads to less guitar-heroesque git history
public static class Rounds 
{
    public static readonly Dictionary<GameRound, RoundDefinition> Definitions = new()
    {
        {
            GameRound.Round0,
            new RoundDefinition
            {
                PoolFillPercent =
                {
                    { EnemyType.RatLeader, 0.35f },
                    { EnemyType.RatFollower, 0.35f }
                }
            }
        },

        {
            GameRound.Round1,
            new RoundDefinition
            {
                PoolFillPercent =
                {
                    { EnemyType.RatLeader, 0.40f },
                    { EnemyType.RatFollower, 0.40f },
                    { EnemyType.Bat, 0.20f }
                }
            }
        },

        {
            GameRound.Round2,
            new RoundDefinition
            {
                PoolFillPercent =
                {
                    { EnemyType.Bat, 0.30f },
                    { EnemyType.SlimeLeader, 0.40f },
                    { EnemyType.SlimeFollower, 0.40f }
                }
            }
        }


    };
}
