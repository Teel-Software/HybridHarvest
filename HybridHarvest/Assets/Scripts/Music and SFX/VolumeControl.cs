using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider Slider;
    public GameObject bgmGameObject;
    
    private const string PrefsBGMVolumeKey = "BGMVolume";
    public void Awake()
    {
        bgmGameObject ??= GameObject.FindGameObjectWithTag("GameMusic");
        var audioSource = bgmGameObject.GetComponent<AudioSource>();
        if (PlayerPrefs.HasKey(PrefsBGMVolumeKey))
        {
            audioSource.volume = PlayerPrefs.GetFloat(PrefsBGMVolumeKey);
        }
        Slider.value = audioSource.volume;
    }

    public void ChangeVolume()
    {
        bgmGameObject ??= GameObject.FindGameObjectWithTag("GameMusic");
        var audioSource = bgmGameObject.GetComponent<AudioSource>();
        PlayerPrefs.SetFloat(PrefsBGMVolumeKey, Slider.value);
        audioSource.volume = Slider.value;
    }
}
