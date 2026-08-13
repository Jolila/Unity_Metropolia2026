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
        GameProgressionManager.Instance.OnCurrentRoundChanged
            += HandleRoundChanged;
    }

    private void OnDisable()
    {
        GameProgressionManager.Instance.OnCurrentRoundChanged
            -= HandleRoundChanged;
    }

    private void HandleRoundChanged(GameRound round)
    {
        MusicTrack newTrack = GetMusicTrack(round);

        pendingTransition = GetTransition(
            currentTrack,
            newTrack
        );

        nextTrack = newTrack;

    }

    private MusicTransition GetTransition(
        MusicTrack from, MusicTrack to)
    {
        return (from, to) switch
        {
            (MusicTrack.Round0, MusicTrack.Round1)
                => MusicTransition.Round0To1,

            (MusicTrack.Round1, MusicTrack.Round2)
                => MusicTransition.Round1To2,

        };
    }

    private MusicTrack GetMusicTrack(GameRound round)
    {
        return round switch
        {
            GameRound.Round0 => MusicTrack.Round0,
            GameRound.Round1 => MusicTrack.Round1,
            GameRound.Round2 => MusicTrack.Round2,
            GameRound.Round3 => MusicTrack.Round3,
            GameRound.Round4 => MusicTrack.Round4,
            GameRound.Round5 => MusicTrack.Round5,
            GameRound.Round6 => MusicTrack.Round6,
            GameRound.Round7 => MusicTrack.Round7,
            GameRound.Round8 => MusicTrack.Round8,
            GameRound.Round9 => MusicTrack.Round9,
            GameRound.Round10 => MusicTrack.Round10,
            GameRound.Round11 => MusicTrack.Round11,
            GameRound.Round12 => MusicTrack.Round12,
            _ => throw new ArgumentOutOfRangeException(nameof(round))
        };
    }

}
