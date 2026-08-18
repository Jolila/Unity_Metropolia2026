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
        },

        {
        GameRound.Round3,
        new RoundDefinition
        {
            PoolFillPercent =
            {

                    { EnemyType.Zombie, 1.0f },
                    { EnemyType.Bat , 0.2f }
            }
        }
        },

        {
        GameRound.Round4,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                    { EnemyType.Bat, 0.30f },
                    { EnemyType.SlimeLeader, 0.40f },
                    { EnemyType.SlimeFollower, 0.40f },
                    { EnemyType.Ghost, 0.4f }
            }
        }
        },

        {
        GameRound.Round5,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Bat, 0.6f },
                { EnemyType.SlimeLeader, 0.40f },
                { EnemyType.SlimeFollower, 0.40f },
                { EnemyType.Ghost, 0.40f },
                { EnemyType.Zombie, 0.6f }
            }
        }
        },

        {
        GameRound.Round6,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Bat, 0.6f },
                { EnemyType.SlimeLeader, 0.40f },
                { EnemyType.SlimeFollower, 0.40f },
                { EnemyType.Zombie, 0.6f },
                { EnemyType.RatLeader, 0.4f },
                { EnemyType.RatFollower, 0.4f }
            }
        }
        },

        {
        GameRound.Round7,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Bat, 1.0f },
                { EnemyType.Ghost, 1.0f },
                { EnemyType.RatLeader, 0.4f},
                { EnemyType.RatFollower, 0.4f}
            }
        }
        },

        {
        GameRound.Round8,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Ghost, 1.0f},
                { EnemyType.RatLeader, 1.0f},
                { EnemyType.RatFollower, 1.0f}
            }
        }
        },

        {
        GameRound.Round9,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Ghost, 1.0f},
                { EnemyType.RatLeader, 1.0f},
                { EnemyType.RatFollower, 1.0f},
                { EnemyType.SlimeLeader, 0.8f},
                { EnemyType.SlimeFollower, 0.8f}
            }
        }
        },

        {
        GameRound.Round10,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Ghost, 1.0f},
                { EnemyType.RatLeader, 1.0f},
                { EnemyType.RatFollower, 1.0f},
                { EnemyType.SlimeLeader, 0.8f},
                { EnemyType.SlimeFollower, 0.8f},
                { EnemyType.Zombie, 0.6f},
                { EnemyType.Bat, 0.6f}
            }
        }
        },

        {
        GameRound.Round11,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Ghost, 1.0f},
                { EnemyType.RatLeader, 1.0f},
                { EnemyType.RatFollower, 1.0f},
                { EnemyType.SlimeLeader, 0.8f},
                { EnemyType.SlimeFollower, 0.8f},
                { EnemyType.Zombie, 0.6f},
                { EnemyType.Bat, 0.6f}
            }
        }
        },

        {
        GameRound.Round12,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Bat, 1.0f },
                { EnemyType.SlimeLeader, 1.0f },
                { EnemyType.SlimeFollower, 1.0f },
                { EnemyType.Ghost, 1.0f },
                { EnemyType.Zombie, 1.0f },
                { EnemyType.RatLeader, 1.0f },
                { EnemyType.RatFollower, 1.0f }
            }
        }
        },


        {
        GameRound.Boss,
        new RoundDefinition
        {
            PoolFillPercent =
            {
                { EnemyType.Bat, 0.0f}
            }
        }

        }

    };
}
