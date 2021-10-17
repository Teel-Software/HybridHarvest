using System.Linq;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
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
    
    [SerializeField] public Sound[] soundEffects;

    public void Play(SoundEffect soundName)
    {
        var sound = soundEffects.FirstOrDefault(item => item.name.Equals(soundName));
        if (sound is null)
        {
            Debug.Log("ERROR: Sound " + soundName + " not found");
            return;
        }
        Source.PlayOneShot(sound?.clip);
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
