using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SFXManager : MonoBehaviour
{    
    [SerializeField] public Sound[] soundEffects;

    public static AudioSource Source;
    public static GameObject Instance;
    void Awake() 
    {
        if (Instance == null)
            Instance = gameObject;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Source = gameObject.GetComponent<AudioSource>();
    }

    public void Play(SoundEffect soundName)
    {
        var sound = soundEffects.FirstOrDefault(item => item.name.Equals(soundName));
        if (sound is null)
        {
            throw new KeyNotFoundException("Sound " + soundName + " not found");
        }
        Source.PlayOneShot(sound.clip);
    }

    public void Play(AudioClip soundClip)
    {
        Source.PlayOneShot(soundClip);
    }
}

public enum SoundEffect
{
    PlantSeed,
}
