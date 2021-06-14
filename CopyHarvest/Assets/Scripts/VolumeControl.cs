using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider Slide;
    public GameObject GameMusic;

    public void Start()
    {
        GameMusic = GameObject.FindGameObjectWithTag("GameMusic");
        var audioSource = GameMusic.GetComponent<AudioSource>();
        Slide.value = audioSource.volume;
    }

    public void ChangeVolume()
    {
        GameMusic = GameObject.FindGameObjectWithTag("GameMusic");
        var audioSource = GameMusic.GetComponent<AudioSource>();
        audioSource.volume = Slide.value;
    }
}
