using UnityEngine;
using UnityEngine.Audio;

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
    [SerializeField] AudioClip _music;

    AudioMixerGroup _musicGroup;

    [Header("Mixer")]
    [SerializeField] private AudioMixerGroup _sfxGroup;

    [Header("Sources")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private int _sfxSourceCount = 12;

    [Header("Clips")]
    [SerializeField] private AudioClip projectileShoot;
    [SerializeField] private AudioClip enemyHit;
    [SerializeField] private AudioClip regularEnemyDeath;
    [SerializeField] private AudioClip ghostDeath;
    [SerializeField] private AudioClip fireRing;
  

    private AudioSource[] _sfxSources;
    private int _nextSource;
    private int currentSource;


    const string MASTER_VOLUME_NAME = "MasterVolume";
    const string MUSIC_VOLUME_NAME = "MusicVolume";
    const string SFX_VOLUME_NAME = "SFXVolume";
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
    }

    void Start()
    {
        PlayMusic();
    }

    void PlayMusic()
    {
        if(_musicSource == null)
        {
            return;
        }

        if(_musicSource.isPlaying)
        {
            return;
        }
        _musicSource.Play();
    }

    public void RestartMusic()
    {
        _musicSource.Stop();
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
        PlayerPrefs.Save();
    }


    public void ChangeSFXVolume(float volume)
    {
        _mixer.SetFloat(SFX_VOLUME_NAME, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Settings.SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void PlayAudio(AudioClip audioClip, SoundType soundType, float volume, bool loop)
    {
        

        if(soundType == SoundType.Music)
        {
            _musicSource.clip = audioClip;
            _musicSource.volume = volume;
            _musicSource.loop = loop;

            _musicSource.Stop();
            _musicSource.Play();

            return;
        }


        GameObject audioObject = new GameObject(audioClip.name + " Source");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.outputAudioMixerGroup = _sfxGroup;
        audioSource.Play();
        Destroy(audioObject, audioClip.length);
      
       
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
        PlaySFX(enemyHit, 0.8f);
    }

    public void PlayEnemyDeath()
    {
        PlaySFX(regularEnemyDeath, 0.8f);
    }

    public void PlayProjectileShoot()
    {
        PlaySFX(projectileShoot, 0.4f);
    }

    public void PlayFireRing()
    {
        PlaySFX(fireRing, 0.3f);
    }

    public void PlayGhostDeath()
    {
        PlaySFX(ghostDeath, 1.0f);
    }



}

