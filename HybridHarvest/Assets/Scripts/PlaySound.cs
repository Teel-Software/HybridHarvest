using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaySound : MonoBehaviour
{
    [FormerlySerializedAs("Sound")] public AudioClip soundClip;
    
    public void Click()
    {
        FindObjectOfType<SFXManager>().Play(soundClip);
    }
}
