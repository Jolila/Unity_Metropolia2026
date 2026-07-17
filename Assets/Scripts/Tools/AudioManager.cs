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

    private AudioSource _musicSource;
    AudioMixerGroup _musicGroup;
    AudioMixerGroup _sfxGroup;

    const string MUSIC_GROUP_NAME = "Music";
    const string SFX_GROUP_NAME = "SFX";

    const string MASTER_VOLUME_NAME = "MasterVolume";
    const string MUSIC_VOLUME_NAME = "MusiVolume";
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
        _musicGroup = _mixer.FindMatchingGroups(MUSIC_GROUP_NAME)[0];
        _sfxGroup = _mixer.FindMatchingGroups(SFX_GROUP_NAME)[0];


        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.clip = _music;
        _musicSource.outputAudioMixerGroup = _musicGroup;
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

}

