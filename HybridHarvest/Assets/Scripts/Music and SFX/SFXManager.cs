using System;
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

    public AudioClip GetClip(SoundEffect soundID)
    {
        return soundEffects[(int)soundID].Clip;
    }

    public void Play(SoundEffect soundID)
    {
        Sound sound;
        try
        {
            sound = soundEffects[(int)soundID];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogError("Sound " + soundID + " not found");
            return;
        }
        Source.PlayOneShot(sound.Clip);
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
