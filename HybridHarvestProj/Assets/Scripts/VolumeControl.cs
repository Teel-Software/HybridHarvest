using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider Slide;
    public AudioSource Music;

    public void Start()
    {
        Slide.value = Music.volume;
    }

    public void ChangeVolume()
    {
        Music.volume = Slide.value;
    }
}
