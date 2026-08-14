using System;
using UnityEngine;

public enum MusicTrack
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
    Round12,
    Boss
}

public enum MusicTransition
{
    Round0To1,
    Round1To2,
    Round2To3,
    Round3To4,
    Round4To5,
    Round5To6,
    Round6To7,
    Round7To8,
    Round8To9,
    Round9To10,
    Round10To11,
    Round11To12,
    Round12ToBoss
}

public class MusicProgressionManager : MonoBehaviour
{
    private static MusicProgressionManager _instance;
    private MusicTrack currentTrack;


    public static MusicProgressionManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindAnyObjectByType<MusicProgressionManager>();

            return _instance;
        }
    }

    private void OnEnable()
    {
       
      
    }

    public void Initialize()
    {
        GameProgressionManager.Instance.OnCurrentRoundChanged
           += HandleRoundChanged;
        currentTrack = MusicTrack.Round0;
    }

    private void OnDisable()
    {
        GameProgressionManager.Instance.OnCurrentRoundChanged
            -= HandleRoundChanged;
    }

   

    private void HandleRoundChanged(GameRound round)
    {

        MusicTrack nextTrack = (MusicTrack)round;
        MusicTransition transition =
            GetMusicTransition(
                round);

        AudioManager.Instance.QueueMusicTransition(
            transition,
            nextTrack
        );

        currentTrack = nextTrack;

    }

    private MusicTransition GetMusicTransition(GameRound round)
    {
        return round switch
        {
            GameRound.Round1 => MusicTransition.Round0To1,
            GameRound.Round2 => MusicTransition.Round1To2,
            GameRound.Round3 => MusicTransition.Round2To3,
            GameRound.Round4 => MusicTransition.Round3To4,
            GameRound.Round5 => MusicTransition.Round4To5,
            GameRound.Round6 => MusicTransition.Round5To6,
            GameRound.Round7 => MusicTransition.Round6To7,
            GameRound.Round8 => MusicTransition.Round7To8,
            GameRound.Round9 => MusicTransition.Round8To9,
            GameRound.Round10 => MusicTransition.Round9To10,
            GameRound.Round11 => MusicTransition.Round10To11,
            GameRound.Round12 => MusicTransition.Round11To12,
            GameRound.Boss => MusicTransition.Round12ToBoss,

            _ => throw new ArgumentOutOfRangeException(nameof(round))
        };
    }


}
