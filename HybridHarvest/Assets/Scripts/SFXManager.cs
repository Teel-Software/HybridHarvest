using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SFXManager : MonoBehaviour
{
    public static AudioSource Source;
    public static GameObject Instance;
    //public string objectTag;
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
}

public enum SoundEffect
{
    PlantSeed,
}
