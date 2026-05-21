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
            Destroy(this);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        _musicGroup = _mixer.FindMatchingGroups(MUSIC_GROUP_NAME)[0];
        _sfxGroup = _mixer.FindMatchingGroups(SFX_GROUP_NAME)[0];

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMasterVolume(float volume)
    {
        _mixer.SetFloat(MASTER_VOLUME_NAME, Mathf.Log10(volume) * 20);
    }

    public void changeMusicVolume(float volume)
    {
        _mixer.SetFloat(MUSIC_VOLUME_NAME, Mathf.Log10(volume) * 20);
    }


    public void ChangeSFXVolume(float volume)
    {
        _mixer.SetFloat(SFX_VOLUME_NAME, Mathf.Log10(volume) * 20);
    }

    public void PlayAudio(AudioClip audioClip, SoundType soundType, float volume, bool loop)
    {
        GameObject newAudioSource = new(audioClip.name + " Source");
        AudioSource audioSource = newAudioSource.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
       

        switch(soundType)
        {
            case SoundType.SFX:
            audioSource.outputAudioMixerGroup = _instance._sfxGroup;
                break;
            case SoundType.Music:
                audioSource.outputAudioMixerGroup = _instance._musicGroup;
                break;
        }
        audioSource.Play();
        if (!loop)
        {
            Destroy(audioSource.gameObject, audioClip.length);
        }
    }

}

