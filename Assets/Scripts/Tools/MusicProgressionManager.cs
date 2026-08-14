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
    private MusicTrack nextTrack;
    private MusicTransition pendingTransition;

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
            (MusicTransition)(int)currentTrack;

        AudioManager.Instance.QueueMusicTransition(
            transition,
            nextTrack
        );

        currentTrack = nextTrack;

    }


}
