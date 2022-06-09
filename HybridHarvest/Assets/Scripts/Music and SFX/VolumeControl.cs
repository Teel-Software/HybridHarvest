using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider SoundSlider;
    public GameObject bgmGameObject;
    public GameObject sfxGameObject;
    [SerializeField] private AudioSource previewSource;
    
    private const string PrefsBGMVolumeKey = "BGMVolume";
    private const string PrefsSFXVolumeKey = "SFXVolume";
    public void Awake()
    {
        bgmGameObject = GameObject.FindGameObjectWithTag("GameMusic");
        sfxGameObject = GameObject.FindGameObjectWithTag("GameSFX");
        
        var bgmSource = bgmGameObject.GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey(PrefsBGMVolumeKey))
        {
            bgmSource.volume = PlayerPrefs.GetFloat(PrefsBGMVolumeKey);
        }
        MusicSlider.value = bgmSource.volume;

        var sfxSource = sfxGameObject.GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey(PrefsSFXVolumeKey))
        {
            sfxSource.volume = PlayerPrefs.GetFloat(PrefsSFXVolumeKey);
        }
        SoundSlider.value = sfxSource.volume;
    }

    public void ChangeMusicVolume()
    {
        bgmGameObject ??= GameObject.FindGameObjectWithTag("GameMusic");
        var audioSource = bgmGameObject.GetComponent<AudioSource>();
        PlayerPrefs.SetFloat(PrefsBGMVolumeKey, MusicSlider.value);
        audioSource.volume = MusicSlider.value;
    }

    public void ChangeSoundVolume()
    {
        sfxGameObject ??= GameObject.FindGameObjectWithTag("GameSFX");
        var audioSource = sfxGameObject.GetComponent<AudioSource>();
        PlayerPrefs.SetFloat(PrefsSFXVolumeKey, SoundSlider.value);
        audioSource.volume = SoundSlider.value;
    }

    public void PlayPreview(bool musicFlag)
    {
        var clip = sfxGameObject.GetComponent<SFXManager>().GetClip(SoundEffect.PlantSeed);
        if (musicFlag)
        {
            previewSource.volume = bgmGameObject.GetComponent<AudioSource>().volume;
        }
        else
        {
            previewSource.volume = sfxGameObject.GetComponent<AudioSource>().volume;
            previewSource.PlayOneShot(clip);
        }
    }
}
