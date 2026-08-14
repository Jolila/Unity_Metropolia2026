using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{

    public enum SoundType
    { SFX, Music}

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    Debug.Log(" Error : no audio manager instance");
                }
            }
            return _instance;
        }
    }

   
    
    [SerializeField] AudioMixer _mixer;
    [SerializeField] AudioSource _musicSource;

    AudioMixerGroup _musicGroup;

    [Header("Mixer")]
    [SerializeField] private AudioMixerGroup _sfxGroup;

    [Header("Sources")]
    [SerializeField] private AudioClip _startupMusic;
    [SerializeField] private AudioClip _countdownClip;
    [SerializeField] private AudioClip[] _musicClips;

    [SerializeField] private AudioClip _round0Music;
    [SerializeField] private AudioClip _round1Music;
    [SerializeField] private AudioClip _round2Music;
    [SerializeField] private AudioClip _round3Music;
    [SerializeField] private AudioClip _round4Music;
    [SerializeField] private AudioClip _round5Music;
    [SerializeField] private AudioClip _round6Music;
    [SerializeField] private AudioClip _round7Music;
    [SerializeField] private AudioClip _round8Music;
    [SerializeField] private AudioClip _round9Music;
    [SerializeField] private AudioClip _round10Music;
    [SerializeField] private AudioClip _round11Music;
    [SerializeField] private AudioClip _round12Music;

    [SerializeField] private AudioClip _round0To1;
    [SerializeField] private AudioClip _round1To2;
    [SerializeField] private AudioClip _round2To3;
    [SerializeField] private AudioClip _round3To4;
    [SerializeField] private AudioClip _round4To5;
    [SerializeField] private AudioClip _round5To6;
    [SerializeField] private AudioClip _round6To7;
    [SerializeField] private AudioClip _round7To8;
    [SerializeField] private AudioClip _round8To9;
    [SerializeField] private AudioClip _round9To10;
    [SerializeField] private AudioClip _round10To11;
    [SerializeField] private AudioClip _round11To12;
    [SerializeField] private AudioClip _round12ToBoss;

    [SerializeField] private AudioClip _bossMusic;

    AudioClip _nextSong;
    AudioClip _currentSong;
    [SerializeField] private int _sfxSourceCount = 12;

    [Header("Clips")]
    [SerializeField] private AudioClip projectileShoot;
    [SerializeField] private AudioClip enemyHit;
    [SerializeField] private AudioClip regularEnemyDeath;
    [SerializeField] private AudioClip ghostDeath;
    [SerializeField] private AudioClip fireRing;
    [SerializeField] private AudioClip onPlayerDeath;
  

    private AudioSource[] _sfxSources;
    private int _nextSource;
    private int currentSource;

    public float musicVolume, SFXVolume, masterVolume;


    const string MASTER_VOLUME_NAME = "MasterVolume";
    const string MUSIC_VOLUME_NAME = "MusicVolume";
    const string SFX_VOLUME_NAME = "SFXVolume";
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private readonly Queue<(AudioClip clip, bool repeat)> _musicQueue = new();

    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _sfxSources = new AudioSource[_sfxSourceCount];

        for (int i = 0; i < _sfxSourceCount; i++)
        {
            GameObject go = new GameObject($"SFX Source {i}");
            go.transform.SetParent(transform);

            AudioSource source = go.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = _sfxGroup;

            _sfxSources[i] = source;
        }
        currentSource = 0;
        _nextSource = 1;

        LoadVolumeSettings();
        PlayStartupSong();
    }

    public void PlayCountDown()
    {
        PlaySFX(_countdownClip, 1.0f);
    }


    public void LoadVolumeSettings()
    {
        float masterVolume = PlayerPrefs.GetFloat("Settings.MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("Settings.MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("Settings.SFXVolume", 1f);

        ChangeMasterVolume(musicVolume);
        ChangeSFXVolume(sfxVolume);
        ChangeMusicVolume(musicVolume);

        this.musicVolume = musicVolume;
        this.SFXVolume = sfxVolume;
        this.masterVolume = masterVolume;
    }

    void Start()
    {

        PlayStartupSong();
        _currentSong = _startupMusic;
        _nextSong = _musicClips[Random.Range(0, _musicClips.Length)];
    }

    public void PlayMusic()
    {

        PlayMusic(_nextSong);
        _currentSong = _nextSong;
        _nextSong = _musicClips[Random.Range(0, _musicClips.Length)];
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }

    void PlayStartupSong()
    {

        if (_musicSource == null)
        {
            return;
        }

        if (_musicSource.isPlaying)
        {
            return;
        }

        _musicSource.clip = _startupMusic;
        _musicSource.loop = false;
        _musicSource.Play();
        
    }

    private void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.Play(); 
    }

  

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMasterVolume(float volume)
    {
        _mixer.SetFloat(MASTER_VOLUME_NAME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Settings.MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void ChangeMusicVolume(float volume)
    {
        _mixer.SetFloat(MUSIC_VOLUME_NAME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Settings.MusicVolume", volume);
        musicVolume = volume;
        PlayerPrefs.Save();
    }


    public void ChangeSFXVolume(float volume)
    {
        _mixer.SetFloat(SFX_VOLUME_NAME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Settings.SFXVolume", volume);
        SFXVolume = volume;
        PlayerPrefs.Save();
    }

    public void PlayAudio(AudioClip audioClip, SoundType soundType, float volume, bool loop)
    {
        

        //GameObject audioObject = new GameObject(audioClip.name + " Source");
        //AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        //audioSource.clip = audioClip;
        //audioSource.volume = volume;
        //audioSource.loop = false;
        //audioSource.outputAudioMixerGroup = _sfxGroup;
        //audioSource.Play();
        //Destroy(audioObject, audioClip.length);
      
       
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        AudioSource source = _sfxSources[currentSource];

        source.clip = clip;
        source.volume = volume;
        source.loop = false;
        source.Play();

        currentSource++;

        if (currentSource >= _sfxSources.Length)
            currentSource = 0;
    }


    public void PlayEnemyHit()
    {
        PlaySFX(enemyHit, 0.4f);
    }

    public void PlayEnemyDeath()
    {
        PlaySFX(regularEnemyDeath, 0.4f);
    }

    public void PlayProjectileShoot()
    {
        PlaySFX(projectileShoot, 0.6f);
    }

    public void PlayFireRing()
    {
        PlaySFX(fireRing, 0.6f);
    }

    public void PlayGhostDeath()
    {
        PlaySFX(ghostDeath, 0.8f);
    }

    public void PlayOnPlayerDeath()
    {
        PlaySFX(onPlayerDeath, 1.0f);
    }

    public void AddToMusicQueue(AudioClip clip, bool repeat)
    {
        if (clip == null)
        {
            Debug.LogWarning("Tried to add null music clip to queue.");
            return;
        }

        _musicQueue.Enqueue((clip, repeat));

        // If nothing is currently playing, start immediately.
        if (!_musicSource.isPlaying)
            PlayNextMusicInQueue();
    }

    private void PlayNextMusicInQueue()
    {
        if (_musicQueue.Count == 0)
            return;

        var next = _musicQueue.Dequeue();

        _musicSource.clip = next.clip;
        _musicSource.loop = next.repeat;
        _musicSource.Play();
    }

    public void QueueMusicTransition(
        MusicTransition transition,
        MusicTrack nextTrack)
    {
        AudioClip transitionClip =
            GetMusicTransition(transition);

        AudioClip nextTrackClip =
            GetMusicTrack(nextTrack);
        Debug.Log(
        $"[Audio] Queueing music transition: " +
        $"{transition} -> {nextTrack}"
        );

        Debug.Log(
            $"[Audio] Transition clip: " +
            $"{(transitionClip != null ? transitionClip.name : "NULL")}"
        );

        Debug.Log(
            $"[Audio] Track clip: " +
            $"{(nextTrackClip != null ? nextTrackClip.name : "NULL")}"
        );


        AddToMusicQueue(
            transitionClip,
            false
        );

        AddToMusicQueue(
            nextTrackClip,
            true
        );
    }

    private AudioClip GetMusicTrack(MusicTrack track)
    {
        return track switch
        {
            MusicTrack.Round0 => _round0Music,
            MusicTrack.Round1 => _round1Music,
            MusicTrack.Round2 => _round2Music,
            MusicTrack.Round3 => _round3Music,
            MusicTrack.Round4 => _round4Music,
            MusicTrack.Round5 => _round5Music,
            MusicTrack.Round6 => _round6Music,
            MusicTrack.Round7 => _round7Music,
            MusicTrack.Round8 => _round8Music,
            MusicTrack.Round9 => _round9Music,
            MusicTrack.Round10 => _round10Music,
            MusicTrack.Round11 => _round11Music,
            MusicTrack.Round12 => _round12Music,
            MusicTrack.Boss => _bossMusic,

            _ => null
        };
    }

    private AudioClip GetMusicTransition(MusicTransition transition)
    {
        return transition switch
        {
            MusicTransition.Round0To1 => _round0To1,
            MusicTransition.Round1To2 => _round1To2,
            MusicTransition.Round2To3 => _round2To3,
            MusicTransition.Round3To4 => _round3To4,
            MusicTransition.Round4To5 => _round4To5,
            MusicTransition.Round5To6 => _round5To6,
            MusicTransition.Round6To7 => _round6To7,
            MusicTransition.Round7To8 => _round7To8,
            MusicTransition.Round8To9 => _round8To9,
            MusicTransition.Round9To10 => _round9To10,
            MusicTransition.Round10To11 => _round10To11,
            MusicTransition.Round11To12 => _round11To12,
            MusicTransition.Round12ToBoss => _round12ToBoss,

            _ => null
        };
    }

}

